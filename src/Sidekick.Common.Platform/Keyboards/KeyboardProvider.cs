using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Logging;
using SharpHook.Native;
using Sidekick.Common.Initialization;
using Sidekick.Common.Keybinds;

namespace Sidekick.Common.Platform.Keyboards
{
    public class KeyboardProvider : IKeyboardProvider, IDisposable
    {
        private static readonly Dictionary<KeyCode, string> Keys = new()
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
            { KeyCode.VcPrintScreen,"PrintScreen" },
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

        private static readonly Regex ModifierKeys = new("^(?:Ctrl|Shift|Alt)$");

        private readonly ILogger<KeyboardProvider> logger;
        private readonly IOptions<SidekickConfiguration> configuration;
        private readonly IServiceProvider serviceProvider;
        private readonly IProcessProvider processProvider;

        private bool HasInitialized { get; set; } = false;
        private SimpleGlobalHook? Hook { get; set; }
        private Task? HookTask { get; set; }
        private LogSource? LogSource { get; set; }

        public KeyboardProvider(
            ILogger<KeyboardProvider> logger,
            IOptions<SidekickConfiguration> configuration,
            IServiceProvider serviceProvider,
            IProcessProvider processProvider)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.processProvider = processProvider;
        }

        public event Action<string>? OnKeyDown;

        private Dictionary<string, IKeybindHandler> KeybindHandlers { get; init; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Low;

        /// <inheritdoc/>
        public Task Initialize()
        {
            // We can't initialize twice
            if (HasInitialized)
            {
                return Task.CompletedTask;
            }

            // Initialize keybindings
            KeybindHandlers.Clear();
            foreach (var keybindType in configuration.Value.Keybinds)
            {
                var keybindHandler = (IKeybindHandler)serviceProvider.GetRequiredService(keybindType);
                foreach (var keybind in keybindHandler.GetKeybinds())
                {
                    if (!KeybindHandlers.TryAdd(keybind, keybindHandler))
                    {
                        logger.LogWarning("[Keyboard] Duplicate keybinding detected {0}.", keybind);
                    }
                }
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

            return Task.CompletedTask;
        }

        private Regex IgnoreHookLogs = new Regex("(?:dispatch_mouse_move|hook_get_multi_click_time|dispatch_event|win_hook_event_proc|dispatch_mouse_wheel|dispatch_button_press|dispatch_button_release)", RegexOptions.Compiled);

        private void OnMessageLogged(object? sender, LogEventArgs e)
        {
            switch (e.LogEntry.Level)
            {
                case SharpHook.Native.LogLevel.Debug:
                    if (IgnoreHookLogs.IsMatch(e.LogEntry.Function))
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

        private void OnKeyPressed(object? sender, KeyboardHookEventArgs args)
        {
            // Make sure the key is one we recognize and validate the event and keybinds
            if (!Keys.TryGetValue(args.RawEvent.Keyboard.KeyCode, out var key)
             || ModifierKeys.IsMatch(key)
             || (!processProvider.IsPathOfExileInFocus && !processProvider.IsSidekickInFocus))
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
            // logger.LogDebug($"[Keyboard] Received key pressed event {keybind}.");

            if (!KeybindHandlers.TryGetValue(keybind, out var keybindHandler) || !keybindHandler.IsValid(keybind))
            {
                return;
            }

            logger.LogDebug($"[Keyboard] Executing keybind handler for {str}.");

            args.SuppressEvent = true;
            Task.Run(async () =>
            {
                await keybindHandler.Execute(keybind);
                logger.LogDebug($"[Keyboard] Completed Keybind Handler for {str}.");
            });
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
                if (ModifierKeys.IsMatch(key))
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

                if (!Keys.Any(x => x.Value == key))
                {
                    continue;
                }

                var validKey = Keys.First(x => x.Value == key);
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
            if (LogSource != null)
            {
                LogSource.Dispose();
                LogSource = null;
            }

            if (Hook != null)
            {
                Hook.KeyPressed -= OnKeyPressed;
                Hook.Dispose();
                Hook = null;
            }

            if (HookTask != null)
            {
                HookTask.Dispose();
                HookTask = null;
            }
        }
    }
}
