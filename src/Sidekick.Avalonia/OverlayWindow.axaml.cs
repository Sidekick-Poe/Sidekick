using System.Text.Json;
using Sidekick.Avalonia.Controls;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class OverlayWindow : BaseWindow
{
    private const int WIDTH = 768;
    private const int HEIGHT = 600;

    /// <summary>
    ///     Cached so that losing focus doesn't read the database after every price check.
    /// </summary>
    private bool closeOnBlur;

    public OverlayWindow():base(HEIGHT, WIDTH, "OverlayWindow")
    {
        InitializeComponent();
        Deactivated += OverlayWindow_Deactivated;

        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        settingsService.OnSettingsChanged += SettingsService_OnSettingsChanged;
        _ = LoadCloseOnBlur();
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
        if (closeOnBlur && IsVisible)
        {
            CloseView();
        }
    }

    private void SettingsService_OnSettingsChanged(string[] keys)
    {
        if (keys.Contains(SettingKeys.OverlayCloseWithMouse))
        {
            _ = LoadCloseOnBlur();
        }
    }

    private async Task LoadCloseOnBlur()
    {
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        closeOnBlur = await settingsService.GetBool(SettingKeys.OverlayCloseWithMouse);
    }
}
