#if WINDOWS
using System.Runtime.InteropServices;
namespace Sidekick.Avalonia.Utilities;

internal static class WindowsNativeMethods
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
}
#endif
