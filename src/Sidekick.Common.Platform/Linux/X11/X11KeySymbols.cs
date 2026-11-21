using System.Runtime.Versioning;

namespace Sidekick.Common.Platform.Linux.X11;

/// <summary>
/// Turns Sidekick's hotkey texts (like "Ctrl+D") into the X11 key symbol and modifiers to grab.
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
internal static class X11KeySymbols
{
    private static readonly Dictionary<string, nint> KeySymbols = CreateKeySymbols();

    private static readonly (string Name, uint Mask)[] Modifiers =
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
    public static bool TryParse(string keybind, out nint keySymbol, out uint modifiers)
    {
        modifiers = 0;
        var rest = keybind;
        var found = true;
        while (found)
        {
            found = false;
            foreach (var (name, mask) in Modifiers)
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

    /// <summary>
    /// Key symbols from X11/keysymdef.h for the key names of the input provider.
    /// </summary>
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
}
