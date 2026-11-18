#if WINDOWS
using System.Runtime.InteropServices;
namespace Sidekick.Avalonia.Utilities;

internal static class WindowsNativeMethods
{
    /// <summary>
    ///     Makes the native title bar use dark mode.
    /// </summary>
    internal const int DwmwaUseImmersiveDarkMode = 20;

    /// <summary>
    ///     Sets the window border color (Windows 11).
    /// </summary>
    internal const int DwmwaBorderColor = 34;

    /// <summary>
    ///     Sets the title bar background color (Windows 11).
    /// </summary>
    internal const int DwmwaCaptionColor = 35;

    /// <summary>
    ///     Sets the title bar text color (Windows 11).
    /// </summary>
    internal const int DwmwaTextColor = 36;

    /// <summary>
    ///     Color value that removes the window border.
    /// </summary>
    internal const int DwmwaColorNone = unchecked((int)0xFFFFFFFE);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
}
#endif
