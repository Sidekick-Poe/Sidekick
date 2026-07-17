using System.Globalization;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class SplashWindow : Window
{
    private const int WIDTH = 600;
    private const int HEIGHT = 500;

    private IServiceProvider ServiceProvider => App.RequiredServerAppHost.Application.Services;

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
    }

    public async Task OpenView(string url)
    {
        await Dispatcher.Invoke(async () =>
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

            await NormalizeView();
        });
    }

    private async Task NormalizeView()
    {
        WindowState = WindowState.Normal;
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        Height = HEIGHT * zoom;
        Width = WIDTH * zoom;
        MinWidth = Width;
        MinHeight = Height;
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
    
    private void WebView_OnEnvironmentRequested(object? sender, WebViewEnvironmentRequestedEventArgs args)
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
    }
}
