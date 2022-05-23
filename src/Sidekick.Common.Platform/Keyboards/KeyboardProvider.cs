using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpHook;
using SharpHook.Native;
using Sidekick.Common.Platform.Keybinds;

namespace Sidekick.Common.Platform.Keyboards
{
    public class KeyboardProvider : IKeyboardProvider, IDisposable
    {
        private const string ALT = "Alt";
        private const string CTRL = "Ctrl";
        private const string SHIFT = "Shift";

        private static readonly List<Key> ValidKeys = new()
        {
            new Key(KeyCode.VcLeftShift, SHIFT, "Shift"),
            new Key(KeyCode.VcRightShift, SHIFT, "Shift"),
            new Key(KeyCode.VcLeftControl, CTRL, "CommandOrControl"),
            new Key(KeyCode.VcRightControl, CTRL, "CommandOrControl"),
            new Key(KeyCode.VcLeftAlt, ALT, "Alt"),
            new Key(KeyCode.VcRightAlt, ALT, "Alt"),

            new Key(KeyCode.VcF1, "F1", "F1"),
            new Key(KeyCode.VcF2, "F2", "F2"),
            new Key(KeyCode.VcF3, "F3", "F3"),
            new Key(KeyCode.VcF4, "F4", "F4"),
            new Key(KeyCode.VcF5, "F5", "F5"),
            new Key(KeyCode.VcF6, "F6", "F6"),
            new Key(KeyCode.VcF7, "F7", "F7"),
            new Key(KeyCode.VcF8, "F8", "F8"),
            new Key(KeyCode.VcF9, "F9", "F9"),
            new Key(KeyCode.VcF10, "F10", "F10"),
            new Key(KeyCode.VcF11, "F11", "F11"),
            new Key(KeyCode.VcF12, "F12", "F12"),
            new Key(KeyCode.VcF13, "F13", "F13"),
            new Key(KeyCode.VcF14, "F14", "F14"),
            new Key(KeyCode.VcF15, "F15", "F15"),
            new Key(KeyCode.VcF16, "F16", "F16"),
            new Key(KeyCode.VcF17, "F17", "F17"),
            new Key(KeyCode.VcF18, "F18", "F18"),
            new Key(KeyCode.VcF19, "F19", "F19"),
            new Key(KeyCode.VcF20, "F20", "F20"),
            new Key(KeyCode.VcF21, "F21", "F21"),
            new Key(KeyCode.VcF22, "F22", "F22"),
            new Key(KeyCode.VcF23, "F23", "F23"),
            new Key(KeyCode.VcF24, "F24", "F24"),

            new Key(KeyCode.Vc0, "0", "0"),
            new Key(KeyCode.Vc1, "1", "1"),
            new Key(KeyCode.Vc2, "2", "2"),
            new Key(KeyCode.Vc3, "3", "3"),
            new Key(KeyCode.Vc4, "4", "4"),
            new Key(KeyCode.Vc5, "5", "5"),
            new Key(KeyCode.Vc6, "6", "6"),
            new Key(KeyCode.Vc7, "7", "7"),
            new Key(KeyCode.Vc8, "8", "8"),
            new Key(KeyCode.Vc9, "9", "9"),

            new Key(KeyCode.VcA, "A", "A"),
            new Key(KeyCode.VcB, "B", "B"),
            new Key(KeyCode.VcC, "C", "C"),
            new Key(KeyCode.VcD, "D", "D"),
            new Key(KeyCode.VcE, "E", "E"),
            new Key(KeyCode.VcF, "F", "F"),
            new Key(KeyCode.VcG, "G", "G"),
            new Key(KeyCode.VcH, "H", "H"),
            new Key(KeyCode.VcI, "I", "I"),
            new Key(KeyCode.VcJ, "J", "J"),
            new Key(KeyCode.VcK, "K", "K"),
            new Key(KeyCode.VcL, "L", "L"),
            new Key(KeyCode.VcM, "M", "M"),
            new Key(KeyCode.VcN, "N", "N"),
            new Key(KeyCode.VcO, "O", "O"),
            new Key(KeyCode.VcP, "P", "P"),
            new Key(KeyCode.VcQ, "Q", "Q"),
            new Key(KeyCode.VcR, "R", "R"),
            new Key(KeyCode.VcS, "S", "S"),
            new Key(KeyCode.VcT, "T", "T"),
            new Key(KeyCode.VcU, "U", "U"),
            new Key(KeyCode.VcV, "V", "V"),
            new Key(KeyCode.VcW, "W", "W"),
            new Key(KeyCode.VcX, "X", "X"),
            new Key(KeyCode.VcY, "Y", "Y"),
            new Key(KeyCode.VcZ, "Z", "Z"),

            new Key(KeyCode.VcMinus, "-", "-"),
            new Key(KeyCode.VcEquals, "=", "="),
            new Key(KeyCode.VcComma, ",", ","),
            new Key(KeyCode.VcPeriod, ".", "."),
            new Key(KeyCode.VcSemicolon, ";", ";"),
            new Key(KeyCode.VcSlash, "/", "/"),
            new Key(KeyCode.VcBackquote, "`", "`"),
            new Key(KeyCode.VcOpenBracket, "[", "["),
            new Key(KeyCode.VcBackSlash, "\\", "\\"),
            new Key(KeyCode.VcCloseBracket, "]", "]"),
            new Key(KeyCode.VcQuote, "'", "'"),

            new Key(KeyCode.VcEscape, "Esc", "Escape"),
            new Key(KeyCode.VcTab, "Tab", "Tab"),
            new Key(KeyCode.VcCapsLock, "CapsLock", "Capslock"),
            new Key(KeyCode.VcSpace, "Space", "Space"),
            new Key(KeyCode.VcBackspace, "Backspace", "Backspace"),
            new Key(KeyCode.VcEnter, "Enter", "Return"),
            new Key(KeyCode.VcPrintscreen,"PrintScreen", "PrintScreen"),
            new Key(KeyCode.VcScrollLock, "ScrollLock", "Scrolllock"),
            new Key(KeyCode.VcInsert, "Insert", "Insert"),
            new Key(KeyCode.VcHome, "Home", "Home"),
            new Key(KeyCode.VcDelete, "Delete", "Delete"),
            new Key(KeyCode.VcEnd, "End", "End"),
            new Key(KeyCode.VcPageDown, "PageDown", "PageDown"),
            new Key(KeyCode.VcPageUp, "PageUp", "PageUp"),

            new Key(KeyCode.VcUp, "Up", "Up"),
            new Key(KeyCode.VcDown, "Down", "Down"),
            new Key(KeyCode.VcLeft, "Left", "Left"),
            new Key(KeyCode.VcRight, "Right", "Right"),

            new Key(KeyCode.VcNumLock, "NumLock", "Numlock"),
            new Key(KeyCode.VcNumPad0, "Num0", "num0"),
            new Key(KeyCode.VcNumPad1, "Num1", "num1"),
            new Key(KeyCode.VcNumPad2, "Num2", "num2"),
            new Key(KeyCode.VcNumPad3, "Num3", "num3"),
            new Key(KeyCode.VcNumPad4, "Num4", "num4"),
            new Key(KeyCode.VcNumPad5, "Num5", "num5"),
            new Key(KeyCode.VcNumPad6, "Num6", "num6"),
            new Key(KeyCode.VcNumPad7, "Num7", "num7"),
            new Key(KeyCode.VcNumPad8, "Num8", "num8"),
            new Key(KeyCode.VcNumPad9, "Num9", "num9"),
        };

        private static readonly Regex ModifierKeys = new($"{CTRL}|{SHIFT}|{ALT}");

        private readonly ILogger<KeyboardProvider> logger;
        private readonly IKeybindProvider keybindProvider;

        private bool HasInitialized { get; set; } = false;
        private TaskPoolGlobalHook Hook { get; set; }
        private Task HookTask { get; set; }
        private EventSimulator Simulator { get; set; }

        private bool AltPressed { get; set; } = false;
        private bool CtrlPressed { get; set; } = false;
        private bool ShiftPressed { get; set; } = false;

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

            Hook = new TaskPoolGlobalHook();

            Hook.KeyPressed += OnKeyPressed;
            Hook.KeyReleased += OnKeyReleased;

            HookTask = Hook.RunAsync();
            Simulator = new EventSimulator();

            HasInitialized = true;
        }

        private void OnKeyPressed(object sender, KeyboardHookEventArgs args)
        {
            // Make sure the key is one we recognize
            if (!ValidKeys.Any(x => x.KeyCode == args.Data.KeyCode))
            {
                return;
            }

            // Handle modifier keys and update their flags
            var key = ValidKeys.Find(x => x.KeyCode == args.Data.KeyCode);
            if (ModifierKeys.IsMatch(key.StringValue))
            {
                switch (key.StringValue)
                {
                    case ALT: AltPressed = true; break;
                    case CTRL: CtrlPressed = true; break;
                    case SHIFT: ShiftPressed = true; break;
                }
                return;
            }

            // Transfer the event key to a string to compare to settings
            var str = new StringBuilder();
            if (CtrlPressed)
            {
                str.Append($"{CTRL}+");
            }
            if (ShiftPressed)
            {
                str.Append($"{SHIFT}+");
            }
            if (AltPressed)
            {
                str.Append($"{ALT}+");
            }

            str.Append(key.StringValue);
            var keybind = str.ToString();

            if (OnKeyDown != null && OnKeyDown.GetInvocationList().Length > 0)
            {
                OnKeyDown.Invoke(keybind);
            }
            else if (keybindProvider.KeybindHandlers.TryGetValue(keybind, out var keybindHandler))
            {
                if (keybindHandler.IsValid())
                {
                    args.Reserved = EventReservedValueMask.SuppressEvent;
                    _ = keybindHandler.Execute(keybind);
                    return;
                }
            }
        }

        private void OnKeyReleased(object sender, KeyboardHookEventArgs args)
        {
            if (!ValidKeys.Any(x => x.KeyCode == args.Data.KeyCode))
            {
                return;
            }

            var key = ValidKeys.Find(x => x.KeyCode == args.Data.KeyCode);
            switch (key.StringValue)
            {
                case ALT: AltPressed = false; break;
                case CTRL: CtrlPressed = false; break;
                case SHIFT: ShiftPressed = false; break;
            }
        }

        public void PressKey(params string[] keys)
        {
            foreach (var stroke in keys)
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

                var fetchKeys = FetchKeys(stroke);

                if (fetchKeys.Keys.Count == 0)
                {
                    continue;
                }

                foreach (var modifierKey in fetchKeys.Modifier)
                {
                    Simulator.SimulateKeyPress(modifierKey.KeyCode);
                }

                foreach (var key in fetchKeys.Keys)
                {
                    Simulator.SimulateKeyPress(key.KeyCode);
                }

                foreach (var key in fetchKeys.Keys)
                {
                    Simulator.SimulateKeyRelease(key.KeyCode);
                }

                foreach (var modifierKey in fetchKeys.Modifier)
                {
                    Simulator.SimulateKeyRelease(modifierKey.KeyCode);
                }
            }
        }

        private (List<Key> Modifier, List<Key> Keys) FetchKeys(string stroke)
        {
            var keyCodes = new List<Key>();
            var modifierCodes = new List<Key>();

            foreach (var key in stroke.Split('+'))
            {
                if (!ValidKeys.Any(x => x.StringValue == key))
                {
                    return (null, null);
                }

                var validKey = ValidKeys.Find(x => x.StringValue == key);
                if (ModifierKeys.IsMatch(key))
                {
                    modifierCodes.Add(validKey);
                }
                else
                {
                    keyCodes.Add(validKey);
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
                Hook.KeyPressed += OnKeyPressed;
                Hook.KeyReleased += OnKeyReleased;
                Hook.Dispose();
            }

            if (HookTask != null)
            {
                HookTask.Dispose();
            }
        }

        public string ToElectronAccelerator(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var fetchKey = FetchKeys(key);
            if (fetchKey.Keys != null && fetchKey.Keys.Count == 0)
            {
                return null;
            }

            var result = new StringBuilder();

            foreach (var code in fetchKey.Modifier)
            {
                result.Append($"{code.ElectronAccelerator}+");
            }

            foreach (var code in fetchKey.Keys)
            {
                result.Append($"{code.ElectronAccelerator}+");
            }

            if (result.Length == 0)
            {
                return null;
            }

            return result.ToString()[..^1];
        }
    }
}
