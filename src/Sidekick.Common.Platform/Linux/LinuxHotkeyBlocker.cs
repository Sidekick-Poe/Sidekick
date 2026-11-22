using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Platform.Linux.X11;
using Sidekick.Common.Settings.Input;
using X11Key = Sidekick.Common.Platform.Linux.X11.X11Key;

namespace Sidekick.Common.Platform.Linux;

/// <summary>
/// Keeps Sidekick's hotkeys from reaching the game on Linux, where the global hook can only watch keys. While the game
/// has focus, the hotkeys (and Ctrl+scroll when stash scrolling is on) are grabbed through X11. A grabbed key that no
/// handler uses at that moment, like Esc with no overlay open, still goes to the game.
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
public class LinuxHotkeyBlocker(
    X11Connection x11,
    ProcessProvider processProvider,
    IServiceProvider serviceProvider,
    IOptions<SidekickConfiguration> configuration,
    ILogger<LinuxHotkeyBlocker> logger) : IHotkeyBlocker
{
    /// <summary>
    /// How long grabs stay off after Sidekick simulates a grabbed key, so the X server handles the simulated key before
    /// the grabs come back.
    /// </summary>
    private static readonly TimeSpan SimulatedInputDelay = TimeSpan.FromMilliseconds(100);

    private IReadOnlyList<KeybindHandler> keybindHandlers = [];

    private IReadOnlyList<ScrollWheelHandler> scrollWheelHandlers = [];

    private int suspendCount;

    /// <summary>
    /// Key symbols from X11/keysymdef.h for the key names of the input provider.
    /// </summary>
    private static readonly Dictionary<string, nint> KeySymbols = CreateKeySymbols();

    private static readonly (string Name, uint Mask)[] ModifierMap =
    [
        ("Meta+", Xlib.Mod4Mask),
        ("Ctrl+", Xlib.ControlMask),
        ("Shift+", Xlib.ShiftMask),
        ("Alt+", Xlib.Mod1Mask),
    ];

    /// <summary>
    /// Reads a hotkey text. The modifiers come first, in any order, and the key name is the rest, which can itself
    /// contain a plus sign, like "Ctrl+Num+".
    /// </summary>
    /// <returns>False when the key has no X11 key symbol.</returns>
    private static bool TryParseHotkey(string keybind, out nint keySymbol, out uint modifiers)
    {
        modifiers = 0;
        var rest = keybind;
        var found = true;
        while (found)
        {
            found = false;
            foreach (var (name, mask) in ModifierMap)
            {
                if (rest.Length > name.Length && rest.StartsWith(name, StringComparison.Ordinal))
                {
                    modifiers |= mask;
                    rest = rest[name.Length..];
                    found = true;
                }
            }
        }

        return KeySymbols.TryGetValue(rest, out keySymbol);
    }

    private static Dictionary<string, nint> CreateKeySymbols()
    {
        var symbols = new Dictionary<string, nint>(StringComparer.Ordinal)
        {
            { "Esc", 0xff1b },
            { "`", 0x60 },
            { "-", 0x2d },
            { "=", 0x3d },
            { "Backspace", 0xff08 },
            { "Tab", 0xff09 },
            { "CapsLock", 0xffe5 },
            { "[", 0x5b },
            { "]", 0x5d },
            { "\\", 0x5c },
            { ";", 0x3b },
            { "'", 0x27 },
            { "Enter", 0xff0d },
            { ",", 0x2c },
            { ".", 0x2e },
            { "/", 0x2f },
            { "Space", 0x20 },
            { "<>", 0x3c },
            { "PrintScreen", 0xff61 },
            { "ScrollLock", 0xff14 },
            { "Pause", 0xff13 },
            { "Cancel", 0xff69 },
            { "Help", 0xff6a },
            { "Insert", 0xff63 },
            { "Delete", 0xffff },
            { "Home", 0xff50 },
            { "End", 0xff57 },
            { "PageUp", 0xff55 },
            { "PageDown", 0xff56 },
            { "Up", 0xff52 },
            { "Left", 0xff51 },
            { "Right", 0xff53 },
            { "Down", 0xff54 },
            { "NumLock", 0xff7f },
            { "NumClear", 0xff9d },
            { "Num/", 0xffaf },
            { "Num*", 0xffaa },
            { "Num-", 0xffad },
            { "Num=", 0xffbd },
            { "Num+", 0xffab },
            { "NumEnter", 0xff8d },
            { "Num.", 0xffae },
            { "Num,", 0xffac },
        };

        for (var i = 0; i < 26; i++)
        {
            symbols.Add(((char)('A' + i)).ToString(), 'a' + i);
        }

        for (var i = 0; i <= 9; i++)
        {
            symbols.Add(i.ToString(), '0' + i);
            symbols.Add($"Num{i}", 0xffb0 + i);
        }

        for (var i = 1; i <= 24; i++)
        {
            symbols.Add($"F{i}", 0xffbe + i - 1);
        }

        return symbols;
    }

    /// <inheritdoc/>
    public Task Initialize()
    {
        // A breakpoint hit while the keyboard waits for a grab decision would freeze the keyboard of the whole desktop.
        if (Debugger.IsAttached || !configuration.Value.ApplicationType.SupportsKeybinds())
        {
            return Task.CompletedTask;
        }

        keybindHandlers = configuration.Value.InputHandlers.Select(serviceProvider.GetRequiredService).OfType<KeybindHandler>().ToList();
        scrollWheelHandlers = configuration.Value.InputHandlers.Select(serviceProvider.GetRequiredService).OfType<ScrollWheelHandler>().ToList();

        // The hotkeys are read each time the game gains focus. Changing them happens in Sidekick's own windows, so the
        // game gains focus again afterwards.
        processProvider.GameFocusChanged += ApplyGrabs;
        ApplyGrabs();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ends the grab that is active while the user still holds the hotkey, which would otherwise catch the simulated
    /// keys. The grabs themselves are only released when a simulated key is grabbed (like the Find item hotkey sent
    /// again after a failed copy), so other input, like the next Ctrl+scroll tick, stays blocked meanwhile.
    /// </summary>
    public bool Suspend(IReadOnlyCollection<string> keyStrokes)
    {
        x11.ReleaseActiveKeyboardGrab();

        var release = keyStrokes.Any(stroke => TryParseHotkey(stroke, out var keySymbol, out var modifiers) && x11.IsGrabbed(new X11Key(keySymbol, modifiers)));
        if (!release)
        {
            return false;
        }

        Interlocked.Increment(ref suspendCount);
        x11.ClearGrabs();
        return true;
    }

    /// <inheritdoc/>
    public void Resume()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(SimulatedInputDelay);
            Interlocked.Decrement(ref suspendCount);
            ApplyGrabs();
        });
    }

    private void ApplyGrabs()
    {
        if (processProvider.GameWindow == 0 || Volatile.Read(ref suspendCount) > 0)
        {
            x11.ClearGrabs();
            return;
        }

        // Several hotkey texts can name the same key, like the same combination used by two handlers.
        var keys = new Dictionary<X11Key, List<string>>();
        foreach (var keybind in keybindHandlers.SelectMany(handler => handler.Keybinds).OfType<string>().Distinct())
        {
            if (!TryParseHotkey(keybind, out var keySymbol, out var modifiers))
            {
                logger.LogDebug("[HotkeyBlocker] No X11 key for the hotkey {Keybind}.", keybind);
                continue;
            }

            var key = new X11Key(keySymbol, modifiers);
            if (!keys.TryGetValue(key, out var keybinds))
            {
                keys[key] = keybinds = [];
            }

            keybinds.Add(keybind);
        }

        x11.SetGrabs(keys.Keys, IsScrollUsed(), key => keys.TryGetValue(key, out var keybinds) && keybinds.Any(IsKeybindUsed), IsScrollUsed);
    }

    /// <summary>
    /// Whether a hotkey handler would run for this hotkey now. When none would, the key goes to the game.
    /// </summary>
    private bool IsKeybindUsed(string keybind)
    {
        return keybindHandlers.Any(handler => handler.Keybinds.Contains(keybind) && handler.IsValid(keybind));
    }

    /// <summary>
    /// Ctrl+scroll is only grabbed while stash scrolling is on, so the game keeps its own Ctrl+scroll otherwise.
    /// </summary>
    private bool IsScrollUsed()
    {
        return scrollWheelHandlers.Any(handler => handler.IsEnabled());
    }
}
