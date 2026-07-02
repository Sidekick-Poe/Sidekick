using System.Globalization;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class StandardWindow : Window
{
    private IServiceProvider ServiceProvider => App.ServerAppHost.Application.Services;
    private const int WIDTH = 968;
    private const int HEIGHT = 768;
    private const string POSITION_PREFIX = "StandardWindow";

    public StandardWindow()
    {
        Title = "Sidekick";
        Width = WIDTH;
        Height = HEIGHT;
        Background = new SolidColorBrush(Color.FromRgb(12, 10, 10));
        WindowDecorations = WindowDecorations.None;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = false;
        ShowInTaskbar = true;
        CanResize = true;

        InitializeComponent();

        WebView.EnvironmentRequested += (_, args) =>
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
        if (IsVisible)
        {
            _ = WebView.InvokeScript($"window.location.href = {JsonSerializer.Serialize(url)};");
            Activate();
        }
        else
        {
            Show();
            WebView.Navigate(new Uri(url));
        }

        await NormalizeView();
    }

    private async Task NormalizeView()
    {
        WindowState = WindowState.Normal;
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        Height = HEIGHT * zoom;
        Width = WIDTH * zoom;

        if (await settingsService.GetBool(SettingKeys.SaveWindowPositions))
        {
            var positionX = await settingsService.GetInt($"{POSITION_PREFIX}_PositionX");
            var positionY = await settingsService.GetInt($"{POSITION_PREFIX}_PositionY");
            Position = new PixelPoint(positionX, positionY);
            var height = await settingsService.GetDouble($"{POSITION_PREFIX}_Height");
            var width = await settingsService.GetDouble($"{POSITION_PREFIX}_Width");
            if (height > HEIGHT && width > WIDTH)
            {
                Height = height;
                Width = width;
            }
        }

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

    public void MinimizeView()
    {
        Dispatcher.InvokeAsync(async () =>
        {
            await SavePosition();
            WindowState = WindowState.Minimized;
        });
    }

    public void MaximizeView()
    {
        Dispatcher.InvokeAsync(async () =>
        {
            if (WindowState == WindowState.Normal)
            {
                await SavePosition();
                WindowState = WindowState.Maximized;
            }
            else
            {
                await NormalizeView();
            }
        });
    }

    private async Task SavePosition()
    {
        if (!IsVisible || !CanResize || WindowState == WindowState.Maximized) return;

        try
        {
            var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
            await settingsService.Set($"{POSITION_PREFIX}_Width", Width);
            await settingsService.Set($"{POSITION_PREFIX}_Height", Height);
            await settingsService.Set($"{POSITION_PREFIX}_PositionX", Position.X);
            await settingsService.Set($"{POSITION_PREFIX}_PositionY", Position.Y);
        }
        catch (Exception e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<StandardWindow>>();
            logger.LogError(e, "[MainWindow] Error saving position");
        }
    }
}
