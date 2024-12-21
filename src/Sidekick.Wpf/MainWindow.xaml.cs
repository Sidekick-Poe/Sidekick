using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Helpers;
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

        CenterHelper.Center(this);
        Activate();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        MaximizeHelper.RemoveHook(this);
        ClearFocusOnClosing();

        if (isClosing || !IsVisible || ResizeMode != ResizeMode.CanResize && ResizeMode != ResizeMode.CanResizeWithGrip || WindowState == WindowState.Maximized)
        {
            return;
        }

        try
        {
            var width = (int)ActualWidth;
            var height = (int)ActualHeight;
            _ = viewLocator.ViewPreferenceService.Set(SidekickView?.CurrentView.Key, width, height);
        }
        catch (Exception)
        {
            // If the save fails, we don't want to stop the execution.
        }

        Resources.Remove("services");
        OverlayContainer?.Dispose();
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
            finally
            {
                WebView = null;
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


    private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    #region Code to make sure the focus is lost correctly when the window is closed.

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private IntPtr previousWindowHandle;

    private void ClearFocusOnClosing()
    {
        // Get the handle for the foreground window (current active window)
        var foregroundWindow = GetForegroundWindow();

        // Check if the window currently active belongs to our application
        if (new WindowInteropHelper(this).Handle != foregroundWindow)
        {
            // Just in case, set the focus back to the application
            SetForegroundWindow(new WindowInteropHelper(this).Handle);
        }

        // Get the handle of the currently focused window (likely the WPF window itself)
        previousWindowHandle = GetForegroundWindow();

        // Remove focus explicitly from WebView
        Keyboard.ClearFocus();

        // Optionally, set focus to a parent element or another default element
        FocusManager.SetFocusedElement(this, this);

        // Ensure the WebView is removed from the visual tree
        WebView.Visibility = Visibility.Collapsed;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Explicitly set focus back to the previous window
        if (previousWindowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(previousWindowHandle);
        }
    }

    #endregion

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        MaximizeHelper.AddHook(this);
    }
}
