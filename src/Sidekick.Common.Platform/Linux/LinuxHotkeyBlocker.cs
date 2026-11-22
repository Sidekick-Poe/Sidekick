using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Platform.Linux.X11;
using Sidekick.Common.Settings.Input;

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

        // Turned off until we find a reliable solution: under Proton, the grabbed keys given back (like Esc) never
        // reached the game (#1401, #1402).
        // The hotkeys are read each time the game gains focus. Changing them happens in Sidekick's own windows, so the
        // game gains focus again afterwards.
        // processProvider.GameFocusChanged += ApplyGrabs;
        // ApplyGrabs();
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

        var release = keyStrokes.Any(stroke => X11KeySymbols.TryParse(stroke, out var keySymbol, out var modifiers) && x11.IsGrabbed(new X11Key(keySymbol, modifiers)));
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
            if (!X11KeySymbols.TryParse(keybind, out var keySymbol, out var modifiers))
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
