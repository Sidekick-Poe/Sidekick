using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
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

    public bool IsReady { get; private set; }

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
        if (!Debugger.IsAttached)
        {
            WebView.WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            WebView.WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        }

        // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
        WebView.Visibility = Visibility.Visible;

        // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set a background color. Otherwise, mouse clicks will go through the window.
        Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
        Opacity = 0.01;

        if (!IsReady)
        {
            Activate();
        }

        IsReady = true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        MaximizeHelper.RemoveHook(this);

        if (isClosing || !IsVisible || ResizeMode != ResizeMode.CanResize && ResizeMode != ResizeMode.CanResizeWithGrip || WindowState == WindowState.Maximized)
        {
            return;
        }

        // Save the window position and size.
        try
        {
            var width = (int)ActualWidth;
            var height = (int)ActualHeight;
            var x = (int)Left;
            var y = (int)Top;

            _ = viewLocator.ViewPreferenceService.Set(SidekickView?.CurrentView.Key, width, height, x, y);
        }
        catch (Exception)
        {
            // If the save fails, we don't want to stop the execution.
        }

        Resources.Remove("services");
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

        UnregisterName("Grid");
        UnregisterName("OverlayContainer");
        UnregisterName("TopBorder");
        UnregisterName("WebView");

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

    /// <summary>
    /// Temporarily deactivates the current window.
    /// This is typically used to manage focus-related issues when the main window should lose focus.
    /// </summary>
    /// <remarks>
    /// We use this method typically right before we close the window. Sidekick had an issue for the longest time,
    /// where closing the overlay would focus a random window instead of giving the focus back to the game.
    /// This method ensures the window loses focus before closing. The way this is done is by creating a temporary window and giving focus to that.
    /// Windows magic.</remarks>
    public void Deactivate()
    {
        // Check if the window is still valid and focused
        if (!IsActive)
        {
            return;
        }

        // Create a hidden dummy window to take focus temporarily
        var helperWindow = new Window
        {
            Width = 1,
            Height = 1,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = null,
            Opacity = 0
        };

        // Show the helper window to take focus
        helperWindow.Show();

        // Set focus to the dummy window before closing it
        helperWindow.Activate();
        helperWindow.Close();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        MaximizeHelper.AddHook(this);
    }
}
