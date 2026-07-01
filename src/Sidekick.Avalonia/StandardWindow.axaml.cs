using System.Globalization;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class StandardWindow : Window
{
    private readonly IServiceProvider serviceProvider;
    private const int WIDTH = 968;
    private const int HEIGHT = 768;

    public StandardWindow(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

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

        if (WindowState == WindowState.Normal)
        {
            var settingsService = serviceProvider.GetRequiredService<ISettingsService>();
            var zoomString = await settingsService.GetString(SettingKeys.Zoom);
            if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

            Height = HEIGHT * zoom;
            Width = WIDTH * zoom;
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

    // private async Task SavePosition()
    // {
    //     if (!IsVisible || ViewType == SidekickViewType.Modal || !CanResize || WindowState == WindowState.Maximized) return;

    //     try
    //     {
    //         var viewPreferenceService = Program.ServiceProvider.GetService<IViewPreferenceService>();
    //         if (viewPreferenceService == null) return;

    //         await viewPreferenceService.Set(ViewType.ToString(), (int)Width, (int)Height, Position.X, Position.Y);
    //         logger.LogInformation("[MainWindow] Position saved");
    //     }
    //     catch (Exception e)
    //     {
    //         logger.LogError(e, "[MainWindow] Error saving position");
    //     }
    // }
}
