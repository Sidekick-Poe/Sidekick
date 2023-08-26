using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Blazor.Views;
using Sidekick.Wpf.Services;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly WpfViewLocator viewLocator;

    private IServiceScope Scope { get; set; }

    public MainWindow(WpfViewLocator viewLocator)
    {
        Scope = App.ServiceProvider.CreateScope();

        Resources.Add("services", Scope.ServiceProvider);
        InitializeComponent();
        this.viewLocator = viewLocator;
    }

    internal SidekickView SidekickView { get; set; }

    internal string CurrentWebPath => WebView.WebView.Source.ToString();

    public void Ready()
    {
        // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
        WebView.Visibility = Visibility.Visible;

        // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set a background color. Otherwise mouse clicks will go through the window.
        Background = (Brush?)new BrushConverter().ConvertFrom("#000000");

        InvalidateVisual();
        Focus();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Scope.Dispose();
        viewLocator.Windows.Remove(this);
    }

    protected bool IsClosing = false;

    protected override async void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (IsClosing || !IsVisible)
        {
            return;
        }

        if (ResizeMode != ResizeMode.CanResize && ResizeMode != ResizeMode.CanResizeWithGrip)
        {
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            return;
        }

        try
        {
            await viewLocator.cacheProvider.Set($"view_preference_{SidekickView.Key}", new ViewPreferences()
            {
                Width = (int)ActualWidth,
                Height = (int)ActualHeight,
            });
        }
        catch (Exception) { }

        IsClosing = true;
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);

        if (SidekickView.CloseOnBlur)
        {
            viewLocator.Close(SidekickView);
        }
    }

    private void TopBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}
