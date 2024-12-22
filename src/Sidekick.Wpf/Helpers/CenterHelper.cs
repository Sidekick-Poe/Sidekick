using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sidekick.Wpf.Helpers;

public static class CenterHelper
{
    public static void Center(Window window)
    {
        // Get the window's handle
        var windowHandle = new WindowInteropHelper(window).Handle;

        // Get the screen containing the window
        var currentScreen = Screen.FromHandle(windowHandle);

        // Get the working area of the screen (excluding taskbar, DPI-aware)
        var workingArea = currentScreen.WorkingArea;

        // Get the DPI scaling factor for the monitor
        var dpi = VisualTreeHelper.GetDpi(window);

        // Convert physical pixels (from working area) to WPF device-independent units (DIPs)
        var workingAreaWidthInDips = workingArea.Width / (dpi.PixelsPerInchX / 96.0);
        var workingAreaHeightInDips = workingArea.Height / (dpi.PixelsPerInchY / 96.0);
        var workingAreaLeftInDips = workingArea.Left / (dpi.PixelsPerInchX / 96.0);
        var workingAreaTopInDips = workingArea.Top / (dpi.PixelsPerInchY / 96.0);

        // Get the actual size of the window in DIPs
        var actualWidth = window.Width;
        var actualHeight = window.Height;

        // Calculate centered position within the working area
        var left = workingAreaLeftInDips + (workingAreaWidthInDips - actualWidth) / 2;
        var top = workingAreaTopInDips + (workingAreaHeightInDips - actualHeight) / 2;

        // Set the window's position
        window.Left = left;
        window.Top = top;
    }
}
