using System.Globalization;
using System.Text.Json;
using Avalonia.Controls;
using Sidekick.Avalonia.Controls;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class SplashWindow : BaseWindow
{
    private const int WIDTH = 600;
    private const int HEIGHT = 500;

    public SplashWindow() :
        base(HEIGHT, WIDTH)
    {
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
}
