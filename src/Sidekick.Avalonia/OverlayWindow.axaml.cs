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

    /// <summary>
    /// Price checks need the overlay to open fast, so it is only released after a long break.
    /// </summary>
    protected override TimeSpan? ReleaseDelay => TimeSpan.FromMinutes(15);

    public async Task OpenView(string url)
    {
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var keepGameFocus = settingsService.GetBool(SettingKeys.OverlayKeepGameFocus);

        await Dispatcher.InvokeAsync(async () =>
        {
            if (IsVisible)
            {
                _ = WebView.InvokeScript($"sidekick.app.navigationManager.navigateTo({JsonSerializer.Serialize(url)});");
                if (!keepGameFocus)
                {
                    Activate();
                }
            }
            else
            {
                ShowActivated = !keepGameFocus;
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
        if (settingsService.GetBool(SettingKeys.OverlayCloseWithMouse))
        {
            CloseView();
        }
    }
}
