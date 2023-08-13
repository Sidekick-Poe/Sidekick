using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;

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

        private static readonly Regex ModifierKeys = new("Ctrl|Shift|Alt");

        private readonly ILogger<KeyboardProvider> logger;
        private readonly IKeybindProvider keybindProvider;

        private bool HasInitialized { get; set; } = false;
        private SimpleGlobalHook Hook { get; set; }
        private Task HookTask { get; set; }
        private EventSimulator Simulator { get; set; }

        public KeyboardProvider(
            ILogger<KeyboardProvider> logger,
            IKeybindProvider keybindProvider)
        {
            this.logger = logger;
            this.keybindProvider = keybindProvider;
        }

        public event Action<string> OnKeyDown;

        public void Initialize()
        {
            // We can't initialize twice
            if (HasInitialized)
            {
                return;
            }

            Hook = new();

            Hook.KeyPressed += OnKeyPressed;

            HookTask = Hook.RunAsync();
            Simulator = new EventSimulator();

            HasInitialized = true;
        }

        private void OnKeyPressed(object sender, KeyboardHookEventArgs args)
        {
            // Make sure the key is one we recognize
            if (!Keys.TryGetValue(args.Data.KeyCode, out var key))
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

            if (OnKeyDown != null && OnKeyDown.GetInvocationList().Length > 0)
            {
                OnKeyDown.Invoke(keybind);
            }
            else if (keybindProvider.KeybindHandlers.TryGetValue(keybind, out var keybindHandler))
            {
                if (keybindHandler.IsValid())
                {
                    args.SuppressEvent = true;
                    Task.Run(() => keybindHandler.Execute(keybind));
                }
            }
        }

        public void PressKey(params string[] keyStrokes)
        {
            foreach (var stroke in keyStrokes)
            {
                logger.LogDebug("[Keyboard] Sending " + stroke);

                switch (stroke)
                {
                    case "Copy":
                        PressKey("Ctrl+C");
                        continue;
                    case "Paste":
                        PressKey("Ctrl+V");
                        continue;
                }

                var (modifiers, keys) = FetchKeys(stroke);

                if (keys.Count == 0)
                {
                    continue;
                }

                foreach (var modifierKey in modifiers)
                {
                    Simulator.SimulateKeyPress(modifierKey);
                }

                foreach (var key in keys)
                {
                    Simulator.SimulateKeyPress(key);
                }

                foreach (var key in keys)
                {
                    Simulator.SimulateKeyRelease(key);
                }

                foreach (var modifierKey in modifiers)
                {
                    Simulator.SimulateKeyRelease(modifierKey);
                }
            }
        }

        private (List<KeyCode> Modifiers, List<KeyCode> Keys) FetchKeys(string stroke)
        {
            var keyCodes = new List<KeyCode>();
            var modifierCodes = new List<KeyCode>();

            foreach (var key in stroke.Split('+'))
            {
                if (!Keys.Any(x => x.Value == key))
                {
                    return (null, null);
                }

                var validKey = Keys.First(x => x.Value == key);
                if (ModifierKeys.IsMatch(key))
                {
                    modifierCodes.Add(validKey.Key);
                }
                else
                {
                    keyCodes.Add(validKey.Key);
                }
            }

            if (keyCodes.Count == 0)
            {
                return (null, null);
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
            if (Hook != null)
            {
                Hook.KeyPressed -= OnKeyPressed;
                Hook.Dispose();
            }

            if (HookTask != null)
            {
                HookTask.Dispose();
            }
        }
    }
}
