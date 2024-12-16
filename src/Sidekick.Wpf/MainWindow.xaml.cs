using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Services;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly WpfViewLocator viewLocator;
    private bool isClosing;

    private IServiceScope Scope { get; set; }

    public Guid Id { get; set; }

    public MainWindow(WpfViewLocator viewLocator)
    {
        Scope = App.ServiceProvider.CreateScope();
        Resources.Add("services", Scope.ServiceProvider);
        InitializeComponent();
        this.viewLocator = viewLocator;
    }

    internal SidekickView? SidekickView { get; set; }

    internal string? CurrentWebPath => WebUtility.UrlDecode(WebView.WebView.Source?.ToString());

    public void Ready()
    {
        // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
        WebView.Visibility = Visibility.Visible;

        // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set a background color. Otherwise, mouse clicks will go through the window.
        Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
        Opacity = 0.01;

        CenterOnScreen();
        Activate();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (isClosing || !IsVisible || ResizeMode != ResizeMode.CanResize && ResizeMode != ResizeMode.CanResizeWithGrip || WindowState == WindowState.Maximized)
        {
            return;
        }

        try
        {
            var width = (int)ActualWidth;
            var height = (int)ActualHeight;
            _ = viewLocator.CacheProvider.Set($"view_preference_{SidekickView?.CurrentView.Key}",
                                              new ViewPreferences()
                                              {
                                                  Width = width,
                                                  Height = height,
                                              });
        }
        catch (Exception)
        {
            // If the save fails, we don't want to stop the execution.
        }

        Resources.Remove("services");
        OverlayContainer?.Dispose();
        WebView.Visibility = Visibility.Hidden;
        viewLocator.Windows.Remove(this);
        Scope.Dispose();

        _ = Task.Run(async () =>
        {
            try
            {
                await WebView.WebView.EnsureCoreWebView2Async();
                await WebView.DisposeAsync();
            }
            catch (Exception)
            {
                // If the dispose fails, we don't want to stop the execution.
            }
        });

        isClosing = true;
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);

        if (SidekickView is
            {
                CloseOnBlur: true
            })
        {
            viewLocator.Close(SidekickView);
        }
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        Grid.Margin = WindowState == WindowState.Maximized ? new Thickness(0) : new Thickness(5);
    }

    private void CenterOnScreen()
    {
        // Get the window's handle
        var windowHandle = new WindowInteropHelper(this).Handle;

        // Get the screen containing the window
        var currentScreen = Screen.FromHandle(windowHandle);

        // Get the working area of the screen (excluding taskbar, DPI-aware)
        var workingArea = currentScreen.WorkingArea;

        // Get the DPI scaling factor for the monitor
        var dpi = VisualTreeHelper.GetDpi(this);

        // Convert physical pixels (from working area) to WPF device-independent units (DIPs)
        var workingAreaWidthInDips = workingArea.Width / (dpi.PixelsPerInchX / 96.0);
        var workingAreaHeightInDips = workingArea.Height / (dpi.PixelsPerInchY / 96.0);
        var workingAreaLeftInDips = workingArea.Left / (dpi.PixelsPerInchX / 96.0);
        var workingAreaTopInDips = workingArea.Top / (dpi.PixelsPerInchY / 96.0);

        // Get the actual size of the window in DIPs
        var actualWidth = Width;
        var actualHeight = Height;

        // Calculate centered position within the working area
        var left = workingAreaLeftInDips + (workingAreaWidthInDips - actualWidth) / 2;
        var top = workingAreaTopInDips + (workingAreaHeightInDips - actualHeight) / 2;

        // Set the window's position
        Left = left;
        Top = top;
    }

    private void TopBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }

    // ReSharper disable All

    #region Code to make maximizing the window take the taskbar into account. https: //stackoverflow.com/questions/20941443/properly-maximizing-wpf-window-with-windowstyle-none

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
        if (hwndSource == null)
        {
            return;
        }

        hwndSource.AddHook(HookProc);
    }

    public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_GETMINMAXINFO)
        {
            return IntPtr.Zero;
        }

        // We need to tell the system what our size should be when maximized. Otherwise it will
        // cover the whole screen, including the task bar.
        var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO))!;

        // Adjust the maximized size and position to fit the work area of the correct monitor
        var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MONITORINFO
            {
                cbSize = Marshal.SizeOf(typeof(MONITORINFO)),
            };
            GetMonitorInfo(monitor, ref monitorInfo);
            var rcWorkArea = monitorInfo.rcWork;
            var rcMonitorArea = monitorInfo.rcMonitor;
            mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
            mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
            mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
            mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
        }

        Marshal.StructureToPtr(mmi, lParam, true);

        return IntPtr.Zero;
    }

    private const int WM_GETMINMAXINFO = 0x0024;

    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    #endregion Code to make maximizing the window take the taskbar into account. https: //stackoverflow.com/questions/20941443/properly-maximizing-wpf-window-with-windowstyle-none

    // ReSharper enable All
}
