using System.Text.Json;
using Sidekick.Avalonia.Controls;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class OverlayWindow : BaseWindow
{
    private const int WIDTH = 768;
    private const int HEIGHT = 600;

    public OverlayWindow():base(HEIGHT, WIDTH, "OverlayWindow")
    {
        InitializeComponent();
        Deactivated += OverlayWindow_Deactivated;
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

            await NormalizeView();
        });
    }

    private void OverlayWindow_Deactivated(object? sender, EventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        if (settingsService.GetBoolNow(SettingKeys.OverlayCloseWithMouse))
        {
            CloseView();
        }
    }
}
