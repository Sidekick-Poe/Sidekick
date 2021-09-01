using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Extensions.Logging;
using NeatInput.Windows;
using NeatInput.Windows.Events;
using NeatInput.Windows.Processing.Keyboard.Enums;

namespace Sidekick.Common.Platform.Windows.Keyboards
{
    public class KeyboardProvider : IKeyboardProvider, IDisposable, IKeyboardEventReceiver
    {
        private static readonly List<Key> ValidKeys = new()
        {
            new Key(Keys.ShiftKey, VirtualKeyCode.SHIFT, "Shift", "Shift"),
            new Key(Keys.LShiftKey, VirtualKeyCode.LSHIFT, "Shift", "Shift"),
            new Key(Keys.RShiftKey, VirtualKeyCode.RSHIFT, "Shift", "Shift"),
            new Key(Keys.ControlKey, VirtualKeyCode.CONTROL, "Ctrl", "CommandOrControl"),
            new Key(Keys.LControlKey, VirtualKeyCode.LCONTROL, "Ctrl", "CommandOrControl"),
            new Key(Keys.RControlKey, VirtualKeyCode.RCONTROL, "Ctrl", "CommandOrControl"),
            new Key(Keys.Menu, VirtualKeyCode.MENU, "Alt", "Alt"),
            new Key(Keys.LMenu, VirtualKeyCode.LMENU, "Alt", "Alt"),
            new Key(Keys.RMenu, VirtualKeyCode.RMENU, "Alt", "Alt"),

            new Key(Keys.F1, VirtualKeyCode.F1, "F1", "F1"),
            new Key(Keys.F2, VirtualKeyCode.F2, "F2", "F2"),
            new Key(Keys.F3, VirtualKeyCode.F3, "F3", "F3"),
            new Key(Keys.F4, VirtualKeyCode.F4, "F4", "F4"),
            new Key(Keys.F5, VirtualKeyCode.F5, "F5", "F5"),
            new Key(Keys.F6, VirtualKeyCode.F6, "F6", "F6"),
            new Key(Keys.F7, VirtualKeyCode.F7, "F7", "F7"),
            new Key(Keys.F8, VirtualKeyCode.F8, "F8", "F8"),
            new Key(Keys.F9, VirtualKeyCode.F9, "F9", "F9"),
            new Key(Keys.F10, VirtualKeyCode.F10, "F10", "F10"),
            new Key(Keys.F11, VirtualKeyCode.F11, "F11", "F11"),
            new Key(Keys.F12, VirtualKeyCode.F12, "F12", "F12"),
            new Key(Keys.F13, VirtualKeyCode.F13, "F13", "F13"),
            new Key(Keys.F14, VirtualKeyCode.F14, "F14", "F14"),
            new Key(Keys.F15, VirtualKeyCode.F15, "F15", "F15"),
            new Key(Keys.F16, VirtualKeyCode.F16, "F16", "F16"),

            new Key(Keys.D0, VirtualKeyCode.VK_0, "0", "0"),
            new Key(Keys.D1, VirtualKeyCode.VK_1, "1", "1"),
            new Key(Keys.D2, VirtualKeyCode.VK_2, "2", "2"),
            new Key(Keys.D3, VirtualKeyCode.VK_3, "3", "3"),
            new Key(Keys.D4, VirtualKeyCode.VK_4, "4", "4"),
            new Key(Keys.D5, VirtualKeyCode.VK_5, "5", "5"),
            new Key(Keys.D6, VirtualKeyCode.VK_6, "6", "6"),
            new Key(Keys.D7, VirtualKeyCode.VK_7, "7", "7"),
            new Key(Keys.D8, VirtualKeyCode.VK_8, "8", "8"),
            new Key(Keys.D9, VirtualKeyCode.VK_9, "9", "9"),

            new Key(Keys.A, VirtualKeyCode.VK_A, "A", "A"),
            new Key(Keys.B, VirtualKeyCode.VK_B, "B", "B"),
            new Key(Keys.C, VirtualKeyCode.VK_C, "C", "C"),
            new Key(Keys.D, VirtualKeyCode.VK_D, "D", "D"),
            new Key(Keys.E, VirtualKeyCode.VK_E, "E", "E"),
            new Key(Keys.F, VirtualKeyCode.VK_F, "F", "F"),
            new Key(Keys.G, VirtualKeyCode.VK_G, "G", "G"),
            new Key(Keys.H, VirtualKeyCode.VK_H, "H", "H"),
            new Key(Keys.I, VirtualKeyCode.VK_I, "I", "I"),
            new Key(Keys.J, VirtualKeyCode.VK_J, "J", "J"),
            new Key(Keys.K, VirtualKeyCode.VK_K, "K", "K"),
            new Key(Keys.L, VirtualKeyCode.VK_L, "L", "L"),
            new Key(Keys.M, VirtualKeyCode.VK_M, "M", "M"),
            new Key(Keys.N, VirtualKeyCode.VK_N, "N", "N"),
            new Key(Keys.O, VirtualKeyCode.VK_O, "O", "O"),
            new Key(Keys.P, VirtualKeyCode.VK_P, "P", "P"),
            new Key(Keys.Q, VirtualKeyCode.VK_Q, "Q", "Q"),
            new Key(Keys.R, VirtualKeyCode.VK_R, "R", "R"),
            new Key(Keys.S, VirtualKeyCode.VK_S, "S", "S"),
            new Key(Keys.T, VirtualKeyCode.VK_T, "T", "T"),
            new Key(Keys.U, VirtualKeyCode.VK_U, "U", "U"),
            new Key(Keys.V, VirtualKeyCode.VK_V, "V", "V"),
            new Key(Keys.W, VirtualKeyCode.VK_W, "W", "W"),
            new Key(Keys.X, VirtualKeyCode.VK_X, "X", "X"),
            new Key(Keys.Y, VirtualKeyCode.VK_Y, "Y", "Y"),
            new Key(Keys.Z, VirtualKeyCode.VK_Z, "Z", "Z"),

            new Key(Keys.OemMinus, VirtualKeyCode.OEM_MINUS, "-", "-"),
            new Key(Keys.Oemplus, VirtualKeyCode.OEM_PLUS, "=", "="),
            new Key(Keys.Oemcomma, VirtualKeyCode.OEM_COMMA, ",", ","),
            new Key(Keys.OemPeriod, VirtualKeyCode.OEM_PERIOD, ",", ","),
            new Key(Keys.Oem1, VirtualKeyCode.OEM_1, ";", ";"),
            new Key(Keys.OemSemicolon, VirtualKeyCode.OEM_1, ";", ";"),
            new Key(Keys.Oem2, VirtualKeyCode.OEM_2, "/", "/"),
            new Key(Keys.OemQuestion, VirtualKeyCode.OEM_2, "/", "/"),
            new Key(Keys.Oem3, VirtualKeyCode.OEM_3, "`", "`"),
            new Key(Keys.Oemtilde, VirtualKeyCode.OEM_3, "`", "`"),
            new Key(Keys.Oem4, VirtualKeyCode.OEM_4, "[", "["),
            new Key(Keys.OemOpenBrackets, VirtualKeyCode.OEM_4, "[", "["),
            new Key(Keys.Oem5, VirtualKeyCode.OEM_5, "\\", "\\"),
            new Key(Keys.OemBackslash, VirtualKeyCode.OEM_5, "\\", "\\"),
            new Key(Keys.Oem6, VirtualKeyCode.OEM_6, "]", "]"),
            new Key(Keys.OemCloseBrackets, VirtualKeyCode.OEM_6, "]", "]"),
            new Key(Keys.Oem7, VirtualKeyCode.OEM_7, "'", "'"),
            new Key(Keys.OemQuotes, VirtualKeyCode.OEM_7, "'", "'"),

            new Key(Keys.Escape, VirtualKeyCode.ESCAPE, "Esc", "Escape"),
            new Key(Keys.Tab, VirtualKeyCode.TAB, "Tab", "Tab"),
            new Key(Keys.Capital, VirtualKeyCode.CAPITAL, "CapsLock", "Capslock"),
            new Key(Keys.CapsLock, VirtualKeyCode.CAPITAL, "CapsLock", "Capslock"),
            new Key(Keys.Space, VirtualKeyCode.SPACE, "Space", "Space"),
            new Key(Keys.Back, VirtualKeyCode.BACK, "Backspace", "Backspace"),
            new Key(Keys.Enter, VirtualKeyCode.RETURN, "Enter", "Return"),
            new Key(Keys.Return, VirtualKeyCode.RETURN, "Enter", "Return"),
            new Key(Keys.PrintScreen, VirtualKeyCode.PRINT, "PrintScreen", "PrintScreen"),
            new Key(Keys.Scroll, VirtualKeyCode.SCROLL, "ScrollLock", "Scrolllock"),
            new Key(Keys.Insert, VirtualKeyCode.INSERT, "Insert", "Insert"),
            new Key(Keys.Home, VirtualKeyCode.HOME, "Home", "Home"),
            new Key(Keys.Delete, VirtualKeyCode.DELETE, "Delete", "Delete"),
            new Key(Keys.End, VirtualKeyCode.END, "End", "End"),
            new Key(Keys.PageDown, VirtualKeyCode.NEXT, "PageDown", "PageDown"),
            new Key(Keys.PageUp, VirtualKeyCode.PRIOR, "PageUp", "PageUp"),

            // new Key(Keys.Pause, VirtualKeyCode.PAUSE, "Pause"),
            // new Key(Keys.Cancel, VirtualKeyCode.CANCEL, "Break"),
            // new Key(Keys.Help, VirtualKeyCode.HELP, "Help"),
            // new Key(Keys.Zoom, VirtualKeyCode.ZOOM, "Zoom"),

            new Key(Keys.Up, VirtualKeyCode.UP, "Up", "Up"),
            new Key(Keys.Down, VirtualKeyCode.DOWN, "Down", "Down"),
            new Key(Keys.Left, VirtualKeyCode.LEFT, "Left", "Left"),
            new Key(Keys.Right, VirtualKeyCode.RIGHT, "Right", "Right"),

            new Key(Keys.NumLock, VirtualKeyCode.NUMLOCK, "NumLock", "Numlock"),
            new Key(Keys.NumPad0, VirtualKeyCode.NUMPAD0, "Num0", "num0"),
            new Key(Keys.NumPad1, VirtualKeyCode.NUMPAD1, "Num1", "num1"),
            new Key(Keys.NumPad2, VirtualKeyCode.NUMPAD2, "Num2", "num2"),
            new Key(Keys.NumPad3, VirtualKeyCode.NUMPAD3, "Num3", "num3"),
            new Key(Keys.NumPad4, VirtualKeyCode.NUMPAD4, "Num4", "num4"),
            new Key(Keys.NumPad5, VirtualKeyCode.NUMPAD5, "Num5", "num5"),
            new Key(Keys.NumPad6, VirtualKeyCode.NUMPAD6, "Num6", "num6"),
            new Key(Keys.NumPad7, VirtualKeyCode.NUMPAD7, "Num7", "num7"),
            new Key(Keys.NumPad8, VirtualKeyCode.NUMPAD8, "Num8", "num8"),
            new Key(Keys.NumPad9, VirtualKeyCode.NUMPAD9, "Num9", "num9"),

        };

        private static readonly Regex ModifierKeys = new("Ctrl|Shift|Alt");

        private readonly ILogger<KeyboardProvider> logger;

        private InputSimulator Simulator { get; set; }
        private InputSource InputSource { get; set; }

        public KeyboardProvider(ILogger<KeyboardProvider> logger)
        {
            this.logger = logger;
        }

        public event Action<string> OnKeyDown;

        public void Initialize()
        {
            InputSource = new InputSource(this);
            InputSource.Listen();

            Simulator = new InputSimulator();
        }

        public void Receive(KeyboardEvent @event)
        {
            if (@event.State == KeyStates.Up || !ValidKeys.Any(x => x.HookKey == @event.Key))
            {
                return;
            }

            var key = ValidKeys.Find(x => x.HookKey == @event.Key);
            if (ModifierKeys.IsMatch(key.StringValue))
            {
                return;
            }

            // Transfer the event key to a string to compare to settings
            var str = new StringBuilder();
            if (IsCtrlPressed())
            {
                str.Append($"{ValidKeys.Find(x => x.HookKey == Keys.ControlKey).StringValue }+");
            }
            if (IsShiftPressed())
            {
                str.Append($"{ValidKeys.Find(x => x.HookKey == Keys.ShiftKey).StringValue }+");
            }
            if (IsAltPressed())
            {
                str.Append($"{ValidKeys.Find(x => x.HookKey == Keys.Menu).StringValue }+");
            }

            if (ValidKeys.Any(x => x.HookKey == @event.Key))
            {
                str.Append(key.StringValue);
            }

            OnKeyDown?.Invoke(str.ToString());
        }

        private bool IsCtrlPressed() =>
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.CONTROL) ||
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LCONTROL) ||
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RCONTROL);

        private bool IsShiftPressed() =>
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT) ||
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LSHIFT) ||
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RSHIFT);

        private bool IsAltPressed() =>
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.MENU) ||
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LMENU) ||
            Simulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RMENU);

        public bool IncludesModifier(string input)
        {
            return FetchKeys(input).Modifier.Count > 0;
        }

        public void PressKey(params string[] keys)
        {
            foreach (var stroke in keys)
            {
                logger.LogDebug("[Keyboard] Sending " + stroke);

                switch (stroke)
                {
                    case "Copy":
                        Simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
                        continue;
                    case "Paste":
                        Simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        continue;
                }

                var fetchKeys = FetchKeys(stroke);

                if (fetchKeys.Keys.Count == 0)
                {
                    continue;
                }

                if (fetchKeys.Modifier.Count > 0)
                {
                    Simulator.Keyboard.ModifiedKeyStroke(fetchKeys.Modifier.Select(x => x.SendKey), fetchKeys.Keys.Select(x => x.SendKey));
                }
                else
                {
                    Simulator.Keyboard.KeyPress(fetchKeys.Keys.Select(x => x.SendKey).ToArray());
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
            if (InputSource != null)
            {
                InputSource.Dispose();
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
