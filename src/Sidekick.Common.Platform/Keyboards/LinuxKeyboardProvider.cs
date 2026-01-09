using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Data;
using SharpHook.Logging;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.EventArgs;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Keyboards;

public class LinuxKeyboardProvider
(
    ILogger<LinuxKeyboardProvider> logger,
    IServiceProvider serviceProvider,
    IProcessProvider processProvider,
    IOverlayStateProvider overlayStateProvider,
    ISettingsService settingsService
) : IKeyboardProvider, IDisposable
{
    private static readonly Dictionary<KeyCode, string> keyMappings = new()
    {
        { KeyCode.VcEscape, "Esc" },
        { KeyCode.VcF1, "F1" },
        { KeyCode.VcF2, "F2" },
        { KeyCode.VcF3, "F3" },
        { KeyCode.VcF4, "F4" },
        { KeyCode.VcF5, "F5" },
        { KeyCode.VcF6, "F6" },
        { KeyCode.VcF7, "F7" },
        { KeyCode.VcF8, "F8" },
        { KeyCode.VcF9, "F9" },
        { KeyCode.VcF10, "F10" },
        { KeyCode.VcF11, "F11" },
        { KeyCode.VcF12, "F12" },
        { KeyCode.VcF13, "F13" },
        { KeyCode.VcF14, "F14" },
        { KeyCode.VcF15, "F15" },
        { KeyCode.VcF16, "F16" },
        { KeyCode.VcF17, "F17" },
        { KeyCode.VcF18, "F18" },
        { KeyCode.VcF19, "F19" },
        { KeyCode.VcF20, "F20" },
        { KeyCode.VcF21, "F21" },
        { KeyCode.VcF22, "F22" },
        { KeyCode.VcF23, "F23" },
        { KeyCode.VcF24, "F24" },
        { KeyCode.VcBackQuote, "`" },
        { KeyCode.Vc0, "0" },
        { KeyCode.Vc1, "1" },
        { KeyCode.Vc2, "2" },
        { KeyCode.Vc3, "3" },
        { KeyCode.Vc4, "4" },
        { KeyCode.Vc5, "5" },
        { KeyCode.Vc6, "6" },
        { KeyCode.Vc7, "7" },
        { KeyCode.Vc8, "8" },
        { KeyCode.Vc9, "9" },
        { KeyCode.VcMinus, "-" },
        { KeyCode.VcEquals, "=" },
        { KeyCode.VcBackspace, "Backspace" },
        { KeyCode.VcTab, "Tab" },
        { KeyCode.VcCapsLock, "CapsLock" },
        { KeyCode.VcA, "A" },
        { KeyCode.VcB, "B" },
        { KeyCode.VcC, "C" },
        { KeyCode.VcD, "D" },
        { KeyCode.VcE, "E" },
        { KeyCode.VcF, "F" },
        { KeyCode.VcG, "G" },
        { KeyCode.VcH, "H" },
        { KeyCode.VcI, "I" },
        { KeyCode.VcJ, "J" },
        { KeyCode.VcK, "K" },
        { KeyCode.VcL, "L" },
        { KeyCode.VcM, "M" },
        { KeyCode.VcN, "N" },
        { KeyCode.VcO, "O" },
        { KeyCode.VcP, "P" },
        { KeyCode.VcQ, "Q" },
        { KeyCode.VcR, "R" },
        { KeyCode.VcS, "S" },
        { KeyCode.VcT, "T" },
        { KeyCode.VcU, "U" },
        { KeyCode.VcV, "V" },
        { KeyCode.VcW, "W" },
        { KeyCode.VcX, "X" },
        { KeyCode.VcY, "Y" },
        { KeyCode.VcZ, "Z" },
        { KeyCode.VcOpenBracket, "[" },
        { KeyCode.VcCloseBracket, "]" },
        { KeyCode.VcBackslash, "\\" },
        { KeyCode.VcSemicolon, ";" },
        { KeyCode.VcQuote, "'" },
        { KeyCode.VcEnter, "Enter" },
        { KeyCode.VcComma, "," },
        { KeyCode.VcPeriod, "." },
        { KeyCode.VcSlash, "/" },
        { KeyCode.VcSpace, "Space" },
        { KeyCode.Vc102, "<>" },
        { KeyCode.VcMisc, "Misc" },
        { KeyCode.VcPrintScreen, "PrintScreen" },
        { KeyCode.VcScrollLock, "ScrollLock" },
        { KeyCode.VcPause, "Pause" },
        { KeyCode.VcCancel, "Cancel" },
        { KeyCode.VcHelp, "Help" },
        { KeyCode.VcInsert, "Insert" },
        { KeyCode.VcDelete, "Delete" },
        { KeyCode.VcHome, "Home" },
        { KeyCode.VcEnd, "End" },
        { KeyCode.VcPageUp, "PageUp" },
        { KeyCode.VcPageDown, "PageDown" },
        { KeyCode.VcUp, "Up" },
        { KeyCode.VcLeft, "Left" },
        { KeyCode.VcRight, "Right" },
        { KeyCode.VcDown, "Down" },
        { KeyCode.VcNumLock, "NumLock" },
        { KeyCode.VcNumPadClear, "NumClear" },
        { KeyCode.VcNumPadDivide, "Num/" },
        { KeyCode.VcNumPadMultiply, "Num*" },
        { KeyCode.VcNumPadSubtract, "Num-" },
        { KeyCode.VcNumPadEquals, "Num=" },
        { KeyCode.VcNumPadAdd, "Num+" },
        { KeyCode.VcNumPadEnter, "NumEnter" },
        { KeyCode.VcNumPadDecimal, "Num." },
        { KeyCode.VcNumPadSeparator, "Num," },
        { KeyCode.VcNumPad0, "Num0" },
        { KeyCode.VcNumPad1, "Num1" },
        { KeyCode.VcNumPad2, "Num2" },
        { KeyCode.VcNumPad3, "Num3" },
        { KeyCode.VcNumPad4, "Num4" },
        { KeyCode.VcNumPad5, "Num5" },
        { KeyCode.VcNumPad6, "Num6" },
        { KeyCode.VcNumPad7, "Num7" },
        { KeyCode.VcNumPad8, "Num8" },
        { KeyCode.VcNumPad9, "Num9" },
        { KeyCode.VcLeftShift, "Shift" },
        { KeyCode.VcRightShift, "Shift" },
        { KeyCode.VcLeftControl, "Ctrl" },
        { KeyCode.VcRightControl, "Ctrl" },
        { KeyCode.VcLeftAlt, "Alt" },
        { KeyCode.VcRightAlt, "Alt" },
    };

    private static readonly Regex modifierKeys = new("^(?:Ctrl|Shift|Alt)$");

    private static readonly HashSet<string> keybindSettingKeys =
    [
        SettingKeys.KeyClose,
        SettingKeys.KeyFindItems,
        SettingKeys.KeyOpenPriceCheck,
        SettingKeys.KeyOpenWiki,
        SettingKeys.KeyOpenInCraftOfExile,
        SettingKeys.ChatCommands,
        SettingKeys.RegexHotkeys,
        SettingKeys.EscapeClosesOverlays,
    ];

    private static readonly Dictionary<string, string> x11KeysymMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Esc", "Escape" },
        { "Enter", "Return" },
        { "Backspace", "BackSpace" },
        { "CapsLock", "Caps_Lock" },
        { "PageUp", "Page_Up" },
        { "PageDown", "Page_Down" },
        { "PrintScreen", "Print" },
        { "ScrollLock", "Scroll_Lock" },
        { "NumLock", "Num_Lock" },
        { "NumClear", "Clear" },
        { "Num/", "KP_Divide" },
        { "Num*", "KP_Multiply" },
        { "Num-", "KP_Subtract" },
        { "Num+", "KP_Add" },
        { "NumEnter", "KP_Enter" },
        { "Num.", "KP_Decimal" },
        { "Num,", "KP_Separator" },
        { "Num0", "KP_0" },
        { "Num1", "KP_1" },
        { "Num2", "KP_2" },
        { "Num3", "KP_3" },
        { "Num4", "KP_4" },
        { "Num5", "KP_5" },
        { "Num6", "KP_6" },
        { "Num7", "KP_7" },
        { "Num8", "KP_8" },
        { "Num9", "KP_9" },
        { "Num=", "KP_Equal" },
        { "`", "grave" },
        { "-", "minus" },
        { "=", "equal" },
        { "[", "bracketleft" },
        { "]", "bracketright" },
        { "\\", "backslash" },
        { ";", "semicolon" },
        { "'", "apostrophe" },
        { ",", "comma" },
        { ".", "period" },
        { "/", "slash" },
        { "Space", "space" },
    };

    private const int X11KeyPress = 2;
    private const int X11KeyRelease = 3;
    private const int X11KeyPressMask = 1 << 0;
    private const int X11KeyReleaseMask = 1 << 1;
    private const uint X11ShiftMask = 1 << 0;
    private const uint X11LockMask = 1 << 1;
    private const uint X11ControlMask = 1 << 2;
    private const uint X11Mod1Mask = 1 << 3;
    private const uint X11Mod2Mask = 1 << 4;
    private const int X11GrabModeAsync = 1;
    private const int X11GrabModeSync = 0;
    private const int X11AllowAsyncKeyboard = 3;
    private const int X11AllowReplayKeyboard = 5;
    private const uint X11RelevantModifiers = X11ShiftMask | X11ControlMask | X11Mod1Mask;
    private static readonly uint[] X11ModifierVariations =
    [
        0,
        X11LockMask,
        X11Mod2Mask,
        X11LockMask | X11Mod2Mask,
    ];

    private bool HasInitialized { get; set; }

    private readonly ISettingsService settingsService = settingsService;
    private readonly IOverlayStateProvider overlayStateProvider = overlayStateProvider;

    private bool X11GrabberFailed { get; set; }

    private Thread? X11Thread { get; set; }

    private CancellationTokenSource? X11Cancellation { get; set; }

    private AutoResetEvent? X11WakeEvent { get; set; }

    private readonly object x11UpdateLock = new();

    private IReadOnlyList<string> x11PendingKeybinds = Array.Empty<string>();

    private bool x11PendingUpdate;

    private SimpleGlobalHook? Hook { get; set; }

    private Task? HookTask { get; set; }

    private LogSource? LogSource { get; set; }

    public event Action<string>? OnKeyDown;

    public event Action<ScrollEventArgs>? OnScrollDown;

    public event Action<ScrollEventArgs>? OnScrollUp;

    public event Action<DraggedEventArgs>? OnMouseDrag;

    private List<KeybindHandler> KeybindHandlers { get; init; } =
    [
    ];

    private readonly Dictionary<KeyCode, string> pendingKeybinds = new();

    public bool IsCtrlPressed { get; private set; }

    public HashSet<string?> UsedKeybinds => [.. KeybindHandlers.SelectMany(k => k.Keybinds)];

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public Task Initialize()
    {
        // Add missing key codes that were not manually curated.
        // We strip the first two characters of the label as all KeyCodes start with 'Vc', and we can strip that part.
        foreach (var keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (!keyMappings.ContainsKey((KeyCode)keyCode))
            {
                keyMappings.Add((KeyCode)keyCode, ((KeyCode)keyCode).ToString().Substring(2));
            }
        }

        if (Debugger.IsAttached)
        {
            return Task.CompletedTask;
        }

        if (OperatingSystem.IsLinux())
        {
            settingsService.OnSettingsChanged += OnSettingsChanged;
            overlayStateProvider.WidgetsChanged += OnWidgetsChanged;
        }

        RegisterHooks();
        return Task.CompletedTask;
    }

    public void RegisterHooks()
    {
        // Initialize keybindings
        KeybindHandlers.Clear();
        foreach (var keybindType in SidekickConfiguration.InputHandlers)
        {
            var keybindHandler = serviceProvider.GetRequiredService(keybindType) as KeybindHandler;
            if (keybindHandler == null) continue;
            KeybindHandlers.Add(keybindHandler);
        }

        // We can't initialize twice
        if (HasInitialized)
        {
            return;
        }

        // Configure hook logging
        LogSource = LogSource.RegisterOrGet(minLevel: SharpHook.Data.LogLevel.Info);
        LogSource.MessageLogged += OnMessageLogged;

        // Initialize keyboard hook
        Hook = new();
        Hook.KeyPressed += OnKeyPressed;
        Hook.KeyReleased += OnKeyReleased;

        // Initialize mouse hook
        Hook.MouseWheel += OnMouseWheel;

        // Initialize mouse drag hook
        Hook.MouseDragged += OnMouseDragged;

        HookTask = Hook.RunAsync();

        // Make sure we don't run this multiple times
        HasInitialized = true;

        EnsureX11KeyGrabs();
    }

    private readonly Regex ignoreHookLogs = new Regex("(?:dispatch_mouse_move|hook_get_multi_click_time|dispatch_event|win_hook_event_proc|dispatch_mouse_wheel|dispatch_button_press|dispatch_button_release)", RegexOptions.Compiled);

    private void OnMessageLogged(object? sender, LogEventArgs e)
    {
        switch (e.LogEntry.Level)
        {
            case SharpHook.Data.LogLevel.Debug:
                if (ignoreHookLogs.IsMatch(e.LogEntry.Function))
                {
                    break;
                }

                logger.LogDebug("[KeyboardHook] {0}", e.LogEntry.FullText);
                break;

            case SharpHook.Data.LogLevel.Info: logger.LogInformation("[KeyboardHook] {0}", e.LogEntry.FullText); break;

            case SharpHook.Data.LogLevel.Warn: logger.LogWarning("[KeyboardHook] {0}", e.LogEntry.FullText); break;

            case SharpHook.Data.LogLevel.Error: logger.LogError("[KeyboardHook] {0}", e.LogEntry.FullText); break;
        }
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs args)
    {
        UpdateModifierState(args.RawEvent.Keyboard.KeyCode, isPressed: true);

        // Make sure the key is one we recognize and validate the event and keybinds
        if (!keyMappings.TryGetValue(args.RawEvent.Keyboard.KeyCode, out var key)
            || modifierKeys.IsMatch(key)
            || processProvider is
            {
                IsPathOfExileInFocus: false,
                IsSidekickInFocus: false
            })
        {
            return;
        }

        // Transfer the event key to a string to compare to settings
        var str = new StringBuilder();
        if ((args.RawEvent.Mask & EventMask.Ctrl) > 0)
        {
            str.Append("Ctrl+");
        }

        if ((args.RawEvent.Mask & EventMask.Shift) > 0)
        {
            str.Append("Shift+");
        }

        if ((args.RawEvent.Mask & EventMask.Alt) > 0)
        {
            str.Append("Alt+");
        }

        str.Append(key);
        var keybind = str.ToString();
        OnKeyDown?.Invoke(keybind);

        var matchingHandlers = KeybindHandlers
            .Where(keybindHandler => keybindHandler.Keybinds.Contains(keybind) && keybindHandler.IsValid(keybind))
            .ToList();

        if (matchingHandlers.Count == 0)
        {
            return;
        }

        if (OperatingSystem.IsLinux())
        {
            args.SuppressEvent = true;
            lock (pendingKeybinds)
            {
                if (pendingKeybinds.ContainsKey(args.RawEvent.Keyboard.KeyCode))
                {
                    return;
                }

                pendingKeybinds[args.RawEvent.Keyboard.KeyCode] = keybind;
            }

            return;
        }

        foreach (var keybindHandler in matchingHandlers)
        {
            logger.LogDebug($"[Keyboard] Executing keybind handler for {str}.");
            args.SuppressEvent = true;
            Task.Run(async () =>
            {
                await keybindHandler.Execute(keybind);
                logger.LogDebug($"[Keyboard] Completed Keybind Handler for {str}.");
            });
        }
    }

    private void OnKeyReleased(object? sender, KeyboardHookEventArgs args)
    {
        UpdateModifierState(args.RawEvent.Keyboard.KeyCode, isPressed: false);

        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        if (!keyMappings.TryGetValue(args.RawEvent.Keyboard.KeyCode, out var key)
            || modifierKeys.IsMatch(key))
        {
            return;
        }

        string? keybind;
        lock (pendingKeybinds)
        {
            if (!pendingKeybinds.TryGetValue(args.RawEvent.Keyboard.KeyCode, out keybind))
            {
                return;
            }

            pendingKeybinds.Remove(args.RawEvent.Keyboard.KeyCode);
        }

        if (string.IsNullOrEmpty(keybind))
        {
            return;
        }

        var matchingHandlers = KeybindHandlers
            .Where(keybindHandler => keybindHandler.Keybinds.Contains(keybind) && keybindHandler.IsValid(keybind))
            .ToList();

        if (matchingHandlers.Count == 0)
        {
            return;
        }

        foreach (var keybindHandler in matchingHandlers)
        {
            logger.LogDebug($"[Keyboard] Executing keybind handler for {keybind}.");
            args.SuppressEvent = true;
            Task.Run(async () =>
            {
                await keybindHandler.Execute(keybind);
                logger.LogDebug($"[Keyboard] Completed Keybind Handler for {keybind}.");
            });
        }
    }


    private void OnMouseWheel(object? sender, MouseWheelHookEventArgs args)
    {
        var str = new StringBuilder();
        if ((args.RawEvent.Mask & EventMask.Ctrl) > 0)
        {
            str.Append("Ctrl+");
        }

        if ((args.RawEvent.Mask & EventMask.Shift) > 0)
        {
            str.Append("Shift+");
        }

        if ((args.RawEvent.Mask & EventMask.Alt) > 0)
        {
            str.Append("Alt+");
        }

        var keybind = str.ToString();
        if (!string.IsNullOrEmpty(keybind)) keybind = keybind[..^1];

        ScrollEventArgs eventArgs = new()
        {
            Masks = keybind,
        };

        if (args.Data.Rotation > 0)
        {
            OnScrollDown?.Invoke(eventArgs);
        }
        else
        {
            OnScrollUp?.Invoke(eventArgs);
        }

        if (eventArgs.Suppress) args.SuppressEvent = true;
    }

    private void OnMouseDragged(object? sender, MouseHookEventArgs args)
    {
        OnMouseDrag?.Invoke(new(args.Data.X, args.Data.Y));
    }

    private static bool IsEscapeOrSpaceKeybind(string keybind)
    {
        var baseKey = GetBaseKey(keybind);
        return baseKey.Equals("Space", StringComparison.OrdinalIgnoreCase)
            || baseKey.Equals("Esc", StringComparison.OrdinalIgnoreCase)
            || baseKey.Equals("Escape", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetBaseKey(string keybind)
    {
        if (string.IsNullOrWhiteSpace(keybind))
        {
            return string.Empty;
        }

        var parts = keybind.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 0 ? string.Empty : parts[^1];
    }

    public Task PressKey(params string[] keyStrokes)
    {
        var simulator = new EventSimulator();

        if (Hook != null)
        {
            Hook.KeyPressed -= OnKeyPressed;
            Hook.KeyReleased -= OnKeyReleased;
        }

        foreach (var stroke in keyStrokes)
        {
            var normalizedStroke = NormalizeStrokeForCtrlHold(stroke);
            if (string.IsNullOrEmpty(normalizedStroke))
            {
                continue;
            }

            logger.LogDebug("[Keyboard] Sending " + normalizedStroke);

            var (modifiers, keys) = FetchKeys(normalizedStroke);

            if (keys.Count == 0)
            {
                continue;
            }

            foreach (var modifierKey in modifiers)
            {
                simulator.SimulateKeyPress(modifierKey);
            }

            foreach (var key in keys)
            {
                simulator.SimulateKeyPress(key);
            }

            foreach (var key in keys)
            {
                simulator.SimulateKeyRelease(key);
            }

            foreach (var modifierKey in modifiers)
            {
                simulator.SimulateKeyRelease(modifierKey);
            }
        }

        if (Hook != null)
        {
            Hook.KeyPressed += OnKeyPressed;
            Hook.KeyReleased += OnKeyReleased;
        }

        return Task.CompletedTask;
    }

    private static (List<KeyCode> Modifiers, List<KeyCode> Keys) FetchKeys(string stroke)
    {
        var keyCodes = new List<KeyCode>();
        var modifierCodes = new List<KeyCode>();

        foreach (var key in stroke.Split('+'))
        {
            // Modifier keys;
            if (modifierKeys.IsMatch(key))
            {
                var modifierKey = key switch
                {
                    "Shift" => KeyCode.VcLeftShift,
                    "Ctrl" => KeyCode.VcLeftControl,
                    "Alt" => KeyCode.VcLeftAlt,
                    _ => KeyCode.Vc0
                };

                if (modifierKey != KeyCode.Vc0)
                {
                    modifierCodes.Add(modifierKey);
                }

                continue;
            }

            if (keyMappings.All(x => x.Value != key))
            {
                continue;
            }

            var validKey = keyMappings.First(x => x.Value == key);
            keyCodes.Add(validKey.Key);
        }

        if (keyCodes.Count == 0)
        {
            return (new(), new());
        }

        return (modifierCodes, keyCodes);
    }

    private void OnSettingsChanged(string[] keys)
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        if (!keys.Any(keybindSettingKeys.Contains))
        {
            return;
        }

        QueueX11KeyGrabUpdate();
    }

    private void OnWidgetsChanged()
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        QueueX11KeyGrabUpdate();
    }

    // SharpHook cannot suppress key events on Linux, so we rely on XGrabKey to swallow or replay them.
    private void EnsureX11KeyGrabs()
    {
        if (!OperatingSystem.IsLinux() || X11GrabberFailed || X11Thread != null)
        {
            return;
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
        {
            logger.LogWarning("[KeyboardHook] DISPLAY not set; X11 key grabs disabled.");
            X11GrabberFailed = true;
            return;
        }

        X11Cancellation = new CancellationTokenSource();
        X11WakeEvent = new AutoResetEvent(false);
        X11Thread = new Thread(() => RunX11GrabLoop(X11Cancellation.Token))
        {
            IsBackground = true,
            Name = "SidekickX11KeyGrabber"
        };
        X11Thread.Start();
        QueueX11KeyGrabUpdate();
    }

    private void QueueX11KeyGrabUpdate()
    {
        if (X11WakeEvent == null)
        {
            return;
        }

        var keybinds = GetKeybindsForGrabs();
        lock (x11UpdateLock)
        {
            x11PendingKeybinds = keybinds;
            x11PendingUpdate = true;
        }

        X11WakeEvent.Set();
    }

    private List<string> GetKeybindsForGrabs()
    {
        var keybinds = KeybindHandlers
            .SelectMany(handler => handler.Keybinds)
            .Where(keybind => !string.IsNullOrWhiteSpace(keybind))
            .Select(keybind => keybind!.Trim())
            .Where(keybind => !keybind.Contains("MWheel", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (!overlayStateProvider.HasOpenWidgets)
        {
            keybinds = keybinds.Where(keybind => !IsEscapeOrSpaceKeybind(keybind)).ToList();
        }

        return keybinds;
    }

    private void RunX11GrabLoop(CancellationToken token)
    {
        IntPtr display = IntPtr.Zero;
        IntPtr root = IntPtr.Zero;
        var grabbedKeys = new HashSet<(int Keycode, uint Modifiers)>();
        var keybindLookup = new Dictionary<(int Keycode, uint Modifiers), string>();
        var suppressedKeys = new HashSet<(int Keycode, uint Modifiers)>();

        try
        {
            display = XOpenDisplay(IntPtr.Zero);
            if (display == IntPtr.Zero)
            {
                logger.LogWarning("[KeyboardHook] XOpenDisplay failed; X11 key grabs disabled.");
                X11GrabberFailed = true;
                return;
            }

            root = XDefaultRootWindow(display);
            if (root == IntPtr.Zero)
            {
                logger.LogWarning("[KeyboardHook] XDefaultRootWindow failed; X11 key grabs disabled.");
                X11GrabberFailed = true;
                return;
            }

            XSelectInput(display, root, X11KeyPressMask | X11KeyReleaseMask);

            while (!token.IsCancellationRequested)
            {
                if (x11PendingUpdate)
                {
                    IReadOnlyList<string> pending;
                    lock (x11UpdateLock)
                    {
                        if (!x11PendingUpdate)
                        {
                            pending = Array.Empty<string>();
                        }
                        else
                        {
                            pending = x11PendingKeybinds;
                            x11PendingUpdate = false;
                        }
                    }

                    UpdateX11Grabs(display, root, grabbedKeys, keybindLookup, pending);
                }

                if (XPending(display) == 0)
                {
                    X11WakeEvent?.WaitOne(50);
                    continue;
                }

                XNextEvent(display, out var xEvent);
                if (xEvent.Type != X11KeyPress && xEvent.Type != X11KeyRelease)
                {
                    continue;
                }

                var baseModifiers = xEvent.KeyEvent.State & X11RelevantModifiers;
                var combo = ((int)xEvent.KeyEvent.Keycode, baseModifiers);
                var shouldSuppress = false;
                var isTargetFocused = processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus;

                if (xEvent.Type == X11KeyPress)
                {
                    if (isTargetFocused && keybindLookup.TryGetValue(combo, out var keybind))
                    {
                        var shouldHandle = KeybindHandlers
                            .Any(handler => handler.Keybinds.Contains(keybind) && handler.IsValid(keybind));
                        shouldSuppress = shouldHandle;
                        if (shouldSuppress)
                        {
                            suppressedKeys.Add(combo);
                        }

                        if (IsEscapeOrSpaceKeybind(keybind))
                        {
                            logger.LogInformation(
                                "[Keyboard/X11] {Keybind} pressed. focus: sidekick={SidekickFocus} poe={PoeFocus} handle={Handle} suppress={Suppress}",
                                keybind,
                                processProvider.IsSidekickInFocus,
                                processProvider.IsPathOfExileInFocus,
                                shouldHandle,
                                shouldSuppress);
                        }
                    }
                }
                else if (xEvent.Type == X11KeyRelease)
                {
                    if (suppressedKeys.Remove(combo))
                    {
                        shouldSuppress = true;
                    }
                }

                var mode = shouldSuppress ? X11AllowAsyncKeyboard : X11AllowReplayKeyboard;
                XAllowEvents(display, mode, xEvent.KeyEvent.Time);
                XFlush(display);
            }
        }
        catch (DllNotFoundException ex)
        {
            logger.LogWarning(ex, "[KeyboardHook] libX11 not available; X11 key grabs disabled.");
            X11GrabberFailed = true;
        }
        catch (EntryPointNotFoundException ex)
        {
            logger.LogWarning(ex, "[KeyboardHook] libX11 entry points missing; X11 key grabs disabled.");
            X11GrabberFailed = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[KeyboardHook] X11 key grab loop failed.");
            X11GrabberFailed = true;
        }
        finally
        {
            if (display != IntPtr.Zero && root != IntPtr.Zero)
            {
                foreach (var grab in grabbedKeys)
                {
                    XUngrabKey(display, grab.Keycode, grab.Modifiers, root);
                }
            }

            if (display != IntPtr.Zero)
            {
                XCloseDisplay(display);
            }

            X11Thread = null;
        }
    }

    private void UpdateX11Grabs(
        IntPtr display,
        IntPtr root,
        HashSet<(int Keycode, uint Modifiers)> grabbedKeys,
        Dictionary<(int Keycode, uint Modifiers), string> keybindLookup,
        IReadOnlyList<string> keybinds)
    {
        foreach (var grab in grabbedKeys)
        {
            XUngrabKey(display, grab.Keycode, grab.Modifiers, root);
        }

        grabbedKeys.Clear();
        keybindLookup.Clear();

        foreach (var keybind in keybinds)
        {
            if (!TryParseX11Keybind(display, keybind, out var keycode, out var modifiers))
            {
                continue;
            }

            keybindLookup[(keycode, modifiers)] = keybind;

            foreach (var variation in X11ModifierVariations)
            {
                var combined = modifiers | variation;
                XGrabKey(display, keycode, combined, root, true, X11GrabModeAsync, X11GrabModeSync);
                grabbedKeys.Add((keycode, combined));
            }
        }

        XSync(display, false);
    }

    private static bool TryParseX11Keybind(
        IntPtr display,
        string keybind,
        out int keycode,
        out uint modifiers)
    {
        keycode = 0;
        modifiers = 0;

        if (string.IsNullOrWhiteSpace(keybind))
        {
            return false;
        }

        string? keyToken = null;
        foreach (var part in keybind.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= X11ControlMask;
                continue;
            }

            if (part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= X11ShiftMask;
                continue;
            }

            if (part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= X11Mod1Mask;
                continue;
            }

            if (keyToken != null)
            {
                return false;
            }

            keyToken = part;
        }

        if (string.IsNullOrWhiteSpace(keyToken))
        {
            return false;
        }

        var keysym = ResolveX11Keysym(keyToken);
        if (keysym == 0)
        {
            return false;
        }

        keycode = XKeysymToKeycode(display, keysym);
        return keycode != 0;
    }

    private static uint ResolveX11Keysym(string keyToken)
    {
        if (x11KeysymMap.TryGetValue(keyToken, out var mapped))
        {
            return XStringToKeysym(mapped);
        }

        var keysym = XStringToKeysym(keyToken);
        if (keysym != 0)
        {
            return keysym;
        }

        if (keyToken.Length == 1)
        {
            keysym = XStringToKeysym(keyToken.ToLowerInvariant());
            if (keysym != 0)
            {
                return keysym;
            }

            return XStringToKeysym(keyToken.ToUpperInvariant());
        }

        return 0;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        try
        {
            // Dispose of the LogSource
            LogSource?.Dispose();
            LogSource = null;

            // Stop the global hook task if it exists
            if (HookTask != null && Hook != null)
            {
                // Cancel the hook task to ensure it's stopped gracefully
                Hook.Dispose(); // This disposes internal resources of SimpleGlobalHook
                HookTask.Wait(); // Wait until the hook task completes fully
                HookTask.Dispose();
                HookTask = null;
            }

            if (OperatingSystem.IsLinux())
            {
                settingsService.OnSettingsChanged -= OnSettingsChanged;
                overlayStateProvider.WidgetsChanged -= OnWidgetsChanged;
            }

            if (X11Cancellation != null)
            {
                X11Cancellation.Cancel();
                X11WakeEvent?.Set();
                X11Thread?.Join(TimeSpan.FromSeconds(2));
                X11Cancellation.Dispose();
                X11Cancellation = null;
            }

            X11Thread = null;
            X11WakeEvent?.Dispose();
            X11WakeEvent = null;

            if (Hook == null)
            {
                return;
            }

            // Ensure hook itself is set to null
            Hook.KeyPressed -= OnKeyPressed;
            Hook.KeyReleased -= OnKeyReleased;
            Hook.MouseWheel -= OnMouseWheel;
            Hook.MouseDragged -= OnMouseDragged;
            Hook.Dispose();
            Hook = null;
        }
        catch (Exception ex)
        {
            // Log any errors during disposal, as they could be valuable for debugging
            logger.LogError(ex, "[LinuxKeyboardProvider] Error during disposal.");
        }
    }

    public void ReleaseAltModifier()
    {
        var simulator = new EventSimulator();
        simulator.SimulateKeyRelease(KeyCode.VcLeftAlt);
        simulator.SimulateKeyRelease(KeyCode.VcRightAlt);
    }

    private void UpdateModifierState(KeyCode keyCode, bool isPressed)
    {
        if (keyCode == KeyCode.VcLeftControl || keyCode == KeyCode.VcRightControl)
        {
            IsCtrlPressed = isPressed;
        }
    }

    private string? NormalizeStrokeForCtrlHold(string stroke)
    {
        if (!OperatingSystem.IsLinux() || !IsCtrlPressed)
        {
            return stroke;
        }

        var parts = stroke.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            return stroke;
        }

        if (!parts.Any(part => part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase)))
        {
            return stroke;
        }

        var filtered = parts.Where(part => !part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase)).ToList();
        return filtered.Count == 0 ? null : string.Join("+", filtered);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XKeyEvent
    {
        public int Type;
        public IntPtr Serial;
        [MarshalAs(UnmanagedType.Bool)]
        public bool SendEvent;
        public IntPtr Display;
        public IntPtr Window;
        public IntPtr Root;
        public IntPtr Subwindow;
        public IntPtr Time;
        public int X;
        public int Y;
        public int XRoot;
        public int YRoot;
        public uint State;
        public uint Keycode;
        [MarshalAs(UnmanagedType.Bool)]
        public bool SameScreen;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct XEvent
    {
        [FieldOffset(0)]
        public int Type;
        [FieldOffset(0)]
        public XKeyEvent KeyEvent;
    }

    [DllImport("libX11.so.6", EntryPoint = "XOpenDisplay")]
    private static extern IntPtr XOpenDisplay(IntPtr displayName);

    [DllImport("libX11.so.6", EntryPoint = "XCloseDisplay")]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XDefaultRootWindow")]
    private static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XSelectInput")]
    private static extern int XSelectInput(IntPtr display, IntPtr window, int eventMask);

    [DllImport("libX11.so.6", EntryPoint = "XGrabKey")]
    private static extern int XGrabKey(
        IntPtr display,
        int keycode,
        uint modifiers,
        IntPtr grabWindow,
        [MarshalAs(UnmanagedType.Bool)] bool ownerEvents,
        int pointerMode,
        int keyboardMode);

    [DllImport("libX11.so.6", EntryPoint = "XUngrabKey")]
    private static extern int XUngrabKey(
        IntPtr display,
        int keycode,
        uint modifiers,
        IntPtr grabWindow);

    [DllImport("libX11.so.6", EntryPoint = "XKeysymToKeycode")]
    private static extern int XKeysymToKeycode(IntPtr display, uint keysym);

    [DllImport("libX11.so.6", EntryPoint = "XStringToKeysym")]
    private static extern uint XStringToKeysym(string name);

    [DllImport("libX11.so.6", EntryPoint = "XPending")]
    private static extern int XPending(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XNextEvent")]
    private static extern int XNextEvent(IntPtr display, out XEvent xEvent);

    [DllImport("libX11.so.6", EntryPoint = "XAllowEvents")]
    private static extern int XAllowEvents(IntPtr display, int eventMode, IntPtr time);

    [DllImport("libX11.so.6", EntryPoint = "XSync")]
    private static extern int XSync(IntPtr display, [MarshalAs(UnmanagedType.Bool)] bool discard);

    [DllImport("libX11.so.6", EntryPoint = "XFlush")]
    private static extern int XFlush(IntPtr display);
}
