using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sidekick.Wpf.Helpers;

public static class CenterHelper
{
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    private const uint MONITOR_DEFAULTTONEAREST = 2;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    public static void Center(Window window)
    {
        // Get the window's handle
        var windowHandle = new WindowInteropHelper(window).Handle;
        var monitor = MonitorFromWindow(windowHandle, MONITOR_DEFAULTTONEAREST);

        var monitorInfo = new MONITORINFO();
        monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        GetMonitorInfo(monitor, ref monitorInfo);

        // Get the working area in physical pixels
        var workingArea = monitorInfo.rcWork;

        // Get the DPI scaling factor for the monitor
        var dpi = VisualTreeHelper.GetDpi(window);

        // Convert physical pixels (from working area) to WPF device-independent units (DIPs)
        var workingAreaWidthInDips = (workingArea.Right - workingArea.Left) / (dpi.PixelsPerInchX / 96.0);
        var workingAreaHeightInDips = (workingArea.Bottom - workingArea.Top) / (dpi.PixelsPerInchY / 96.0);
        var workingAreaLeftInDips = workingArea.Left / (dpi.PixelsPerInchX / 96.0);
        var workingAreaTopInDips = workingArea.Top / (dpi.PixelsPerInchY / 96.0);

        // Calculate centered position within the working area
        var left = workingAreaLeftInDips + (workingAreaWidthInDips - window.Width) / 2;
        var top = workingAreaTopInDips + (workingAreaHeightInDips - window.Height) / 2;

        // Set the window's position
        window.Left = left;
        window.Top = top;
    }
}
