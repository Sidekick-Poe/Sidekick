using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Sidekick.Wpf.Helpers
{
    /// <summary>
    /// Helper class to position windows to the center.
    /// https://jaybrockway.us/computer-science/wpf-center-window-to-screen/
    /// </summary>
    internal static class WindowPlacement
    {
        /// <summary>
        /// Resize window to fit screen, then center window to screen.
        /// </summary>
        /// <param name="window">Window to be centered.</param>
        public static void ConstrainAndCenterWindowToScreen(Window window)
        {
            if (window == null
                || double.IsNaN(window.ActualHeight)
                || double.IsNaN(window.ActualWidth))
            {
                return;
            }

            var screen = Screen.FromHandle(new WindowInteropHelper(window).Handle);

            // If actual height or width of window is greater than the screen then set height or width.
            if (window.ActualHeight > screen.WorkingArea.Height)
            {
                window.Height = screen.WorkingArea.Height;
                window.MaxHeight = screen.WorkingArea.Height;
            }

            if (window.ActualWidth > screen.WorkingArea.Width)
            {
                window.Width = screen.WorkingArea.Width;
                window.MaxWidth = screen.WorkingArea.Width;
            }

            // Recenter window within the screen.
            // Starting on left side of screen add half width of screen to get to center of screen then
            // subract half width of window to get to left side of window.
            window.Left = screen.WorkingArea.Left + (screen.WorkingArea.Width - window.ActualWidth) / 2;
            window.Top = screen.WorkingArea.Top + (screen.WorkingArea.Height - window.ActualHeight) / 2;
        }

        /// <summary>
        /// Determine host screen of parent window, resize child window to fit screen and center child window to screen.
        /// </summary>
        /// <param name="childWindow">Window to be centered.</param>
        /// <param name="parentWindow">Window to be centered within.</param>
        public static void ConstrainAndCenterWindowToScreenOfParentWindow(Window childWindow, Window parentWindow)
        {
            // Get screen of parent window.
            var screen = Screen.FromHandle(new WindowInteropHelper(parentWindow).Handle);

            // If actual height or width of child window is greater than the screen then set height or width to that of
            // the screen.
            if (childWindow.ActualHeight > screen.WorkingArea.Height)
            {
                childWindow.Height = screen.WorkingArea.Height;
                childWindow.MaxHeight = screen.WorkingArea.Height;
            }

            if (childWindow.ActualWidth > screen.WorkingArea.Width)
            {
                childWindow.Width = screen.WorkingArea.Width;
                childWindow.MaxWidth = screen.WorkingArea.Width;
            }

            // Recenter child window within the screen of the parent window.
            // Starting on left side of screen add half width of screen to get to center of screen then
            // subract half width of window to get to left side of window.
            childWindow.Left = screen.WorkingArea.Left + (screen.WorkingArea.Width - childWindow.ActualWidth) / 2;
            childWindow.Top = screen.WorkingArea.Top + (screen.WorkingArea.Height - childWindow.ActualHeight) / 2;
        }
    }
}
