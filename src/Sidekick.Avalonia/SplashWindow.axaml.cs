using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;

namespace Sidekick.Avalonia;

public partial class SplashWindow : Window
{
    private const int WIDTH = 600;
    private const int HEIGHT = 500;

    public SplashWindow()
    {
        Title = "Sidekick";
        Width = WIDTH;
        Height = HEIGHT;
        Background = new SolidColorBrush(Color.FromRgb(12, 10, 10));
        WindowDecorations = WindowDecorations.None;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = false;
        ShowInTaskbar = true;
        CanResize = false;

        InitializeComponent();

        WebView.EnvironmentRequested += (sender, args) =>
        {
#if DEBUG
            // Enable developer tools for all platforms
            args.EnableDevTools = true;
#endif

            // Platform-specific configuration
            switch (args)
            {
                case WindowsWebView2EnvironmentRequestedEventArgs webView2Args:
                    webView2Args.IsInPrivateModeEnabled = true;
                    break;
                case AppleWKWebViewEnvironmentRequestedEventArgs appleArgs:
                    appleArgs.NonPersistentDataStore = true;
                    break;
                case GtkWebViewEnvironmentRequestedEventArgs gtkArgs:
                    gtkArgs.EphemeralDataManager = true;
                    break;
            }
        };
    }

    public async Task OpenView(string url)
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            if (IsVisible)
            {
                _ = WebView.InvokeScript($"sidekick.app.navigationManager.navigateTo({JsonSerializer.Serialize(url)});");
                Activate();
            }
            else
            {
                Show();
                WebView.Navigate(new Uri(url));
            }
        });
    }

    public void CloseView()
    {
        Hide();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        CloseView();
        e.Cancel = true;
        base.OnClosing(e);
    }
}
