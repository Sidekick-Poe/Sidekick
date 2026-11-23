using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Sidekick.Common.Platform.Windows.DllImport;

[SupportedOSPlatform(PlatformNames.Windows)]
public static class Dwmapi
{
    /// <summary>
    /// Makes the native title bar use dark mode.
    /// </summary>
    public const int DwmwaUseImmersiveDarkMode = 20;

    /// <summary>
    /// Sets the window border color (Windows 11).
    /// </summary>
    public const int DwmwaBorderColor = 34;

    /// <summary>
    /// Sets the title bar background color (Windows 11).
    /// </summary>
    public const int DwmwaCaptionColor = 35;

    /// <summary>
    /// Sets the title bar text color (Windows 11).
    /// </summary>
    public const int DwmwaTextColor = 36;

    /// <summary>
    /// Color value that removes the window border.
    /// </summary>
    public const int DwmwaColorNone = unchecked((int)0xFFFFFFFE);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
}
