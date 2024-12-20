using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Logging;
using SharpHook.Native;
using Sidekick.Common.Keybinds;

namespace Sidekick.Common.Platform.Keyboards
{
    public class KeyboardProvider(
        ILogger<KeyboardProvider> logger,
        IOptions<SidekickConfiguration> configuration,
        IServiceProvider serviceProvider,
        IProcessProvider processProvider) : IKeyboardProvider, IDisposable
    {
        private static readonly Dictionary<KeyCode, string> keyMappings = new()
        {
            { KeyCode.VcLeftShift, "Shift" },
            { KeyCode.VcRightShift, "Shift" },
            { KeyCode.VcLeftControl, "Ctrl" },
            { KeyCode.VcRightControl, "Ctrl" },
            { KeyCode.VcLeftAlt, "Alt" },
            { KeyCode.VcRightAlt, "Alt" },
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
            { KeyCode.VcMinus, "-" },
            { KeyCode.VcEquals, "=" },
            { KeyCode.VcComma, "," },
            { KeyCode.VcPeriod, "." },
            { KeyCode.VcSemicolon, ";" },
            { KeyCode.VcSlash, "/" },
            { KeyCode.VcBackQuote, "`" },
            { KeyCode.VcOpenBracket, "[" },
            { KeyCode.VcBackslash, "\\" },
            { KeyCode.VcCloseBracket, "]" },
            { KeyCode.VcQuote, "'" },
            { KeyCode.VcEscape, "Esc" },
            { KeyCode.VcTab, "Tab" },
            { KeyCode.VcCapsLock, "CapsLock" },
            { KeyCode.VcSpace, "Space" },
            { KeyCode.VcBackspace, "Backspace" },
            { KeyCode.VcEnter, "Enter" },
            { KeyCode.VcPrintScreen, "PrintScreen" },
            { KeyCode.VcScrollLock, "ScrollLock" },
            { KeyCode.VcInsert, "Insert" },
            { KeyCode.VcHome, "Home" },
            { KeyCode.VcDelete, "Delete" },
            { KeyCode.VcEnd, "End" },
            { KeyCode.VcPageDown, "PageDown" },
            { KeyCode.VcPageUp, "PageUp" },
            { KeyCode.VcUp, "Up" },
            { KeyCode.VcDown, "Down" },
            { KeyCode.VcLeft, "Left" },
            { KeyCode.VcRight, "Right" },
            { KeyCode.VcNumLock, "NumLock" },
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
        };

        private static readonly Regex modifierKeys = new("^(?:Ctrl|Shift|Alt)$");

        private bool HasInitialized { get; set; }

        private SimpleGlobalHook? Hook { get; set; }

        private Task? HookTask { get; set; }

        private LogSource? LogSource { get; set; }

        public event Action<string>? OnKeyDown;

        private List<KeybindHandler> KeybindHandlers { get; init; } =
        [
        ];

        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        public Task Initialize()
        {
            if (Debugger.IsAttached)
            {
                return Task.CompletedTask;
            }

            RegisterHooks();
            return Task.CompletedTask;
        }

        public void RegisterHooks()
        {
            // We can't initialize twice
            if (HasInitialized)
            {
                return;
            }

            // Initialize keybindings
            KeybindHandlers.Clear();
            foreach (var keybindType in configuration.Value.Keybinds)
            {
                var keybindHandler = (KeybindHandler)serviceProvider.GetRequiredService(keybindType);
                KeybindHandlers.Add(keybindHandler);
            }

            // Configure hook logging
            LogSource = LogSource.RegisterOrGet(minLevel: SharpHook.Native.LogLevel.Info);
            LogSource.MessageLogged += OnMessageLogged;

            // Initialize keyboard hook
            Hook = new();
            Hook.KeyPressed += OnKeyPressed;
            HookTask = Hook.RunAsync();

            // Make sure we don't run this multiple times
            HasInitialized = true;
        }

        private readonly Regex ignoreHookLogs = new Regex("(?:dispatch_mouse_move|hook_get_multi_click_time|dispatch_event|win_hook_event_proc|dispatch_mouse_wheel|dispatch_button_press|dispatch_button_release)", RegexOptions.Compiled);

        private void OnMessageLogged(
            object? sender,
            LogEventArgs e)
        {
            switch (e.LogEntry.Level)
            {
                case SharpHook.Native.LogLevel.Debug:
                    if (ignoreHookLogs.IsMatch(e.LogEntry.Function))
                    {
                        break;
                    }

                    logger.LogDebug("[KeyboardHook] {0}", e.LogEntry.FullText);
                    break;

                case SharpHook.Native.LogLevel.Info:
                    logger.LogInformation("[KeyboardHook] {0}", e.LogEntry.FullText);
                    break;

                case SharpHook.Native.LogLevel.Warn:
                    logger.LogWarning("[KeyboardHook] {0}", e.LogEntry.FullText);
                    break;

                case SharpHook.Native.LogLevel.Error:
                    logger.LogError("[KeyboardHook] {0}", e.LogEntry.FullText);
                    break;
            }
        }

        private void OnKeyPressed(
            object? sender,
            KeyboardHookEventArgs args)
        {
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
            if ((args.RawEvent.Mask & ModifierMask.Ctrl) > 0)
            {
                str.Append("Ctrl+");
            }

            if ((args.RawEvent.Mask & ModifierMask.Shift) > 0)
            {
                str.Append("Shift+");
            }

            if ((args.RawEvent.Mask & ModifierMask.Alt) > 0)
            {
                str.Append("Alt+");
            }

            str.Append(key);
            var keybind = str.ToString();
            OnKeyDown?.Invoke(keybind);

            foreach (var keybindHandler in KeybindHandlers.Where(keybindHandler => keybindHandler.Keybinds.Contains(keybind) && keybindHandler.IsValid(keybind)))
            {
                logger.LogDebug($"[Keyboard] Executing keybind handler for {str}.");
                args.SuppressEvent = true;
                Task.Run(
                    async () =>
                    {
                        await keybindHandler.Execute(keybind);
                        logger.LogDebug($"[Keyboard] Completed Keybind Handler for {str}.");
                    });
            }
        }

        public Task PressKey(params string[] keyStrokes)
        {
            var simulator = new EventSimulator();

            if (Hook != null)
            {
                Hook.KeyPressed -= OnKeyPressed;
            }

            foreach (var stroke in keyStrokes)
            {
                logger.LogDebug("[Keyboard] Sending " + stroke);

                var (modifiers, keys) = FetchKeys(stroke);

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
            }

            return Task.CompletedTask;
        }

        private (List<KeyCode> Modifiers, List<KeyCode> Keys) FetchKeys(string stroke)
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

                if (Hook == null)
                {
                    return;
                }

                // Ensure hook itself is set to null
                Hook.KeyPressed -= OnKeyPressed;
                Hook.Dispose();
                Hook = null;
            }
            catch (Exception ex)
            {
                // Log any errors during disposal, as they could be valuable for debugging
                logger.LogError(ex, "[KeyboardProvider] Error during disposal.");
            }
        }
    }
}
