using System.Text.Json;
using Sidekick.Avalonia.Controls;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class OverlayWindow : BaseWindow
{
    private const int WIDTH = 768;
    private const int HEIGHT = 600;

    private readonly IInputProvider inputProvider;

    public OverlayWindow():base(HEIGHT, WIDTH, "OverlayWindow")
    {
        InitializeComponent();
        Deactivated += OverlayWindow_Deactivated;

        inputProvider = ServiceProvider.GetRequiredService<IInputProvider>();
        inputProvider.OnMouseDown += InputProvider_OnMouseDown;
    }

    /// <summary>
    /// Price checks need the overlay to open fast, so it is only released after a long break.
    /// </summary>
    protected override TimeSpan? ReleaseDelay => TimeSpan.FromMinutes(15);

    /// <summary>
    /// The game stays focused while the overlay is shown, so the player can keep playing while reading the prices.
    /// Clicking inside the overlay focuses it, for typing in the filters.
    /// </summary>
    public async Task OpenView(string url)
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            if (IsVisible)
            {
                _ = WebView.InvokeScript($"sidekick.app.navigationManager.navigateTo({JsonSerializer.Serialize(url)});");
            }
            else
            {
                ShowActivated = false;
                Show();
                WebView.Navigate(new Uri(url));
            }

            await NormalizeView();
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        inputProvider.OnMouseDown -= InputProvider_OnMouseDown;
        base.OnClosed(e);
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

    /// <summary>
    /// Closes the overlay on a click outside it. The Deactivated event only covers an overlay the user clicked into, since
    /// the game keeps the focus until then.
    /// </summary>
    private void InputProvider_OnMouseDown(int x, int y)
    {
        Dispatcher.Post(() =>
        {
            if (!IsVisible || ContainsScreenPoint(x, y))
            {
                return;
            }

            var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
            if (settingsService.GetBool(SettingKeys.OverlayCloseWithMouse))
            {
                CloseView();
            }
        });
    }

    /// <summary>
    /// Position is in screen pixels already, while sizes are in device independent units, so they are scaled to match
    /// the mouse position from the global hook.
    /// </summary>
    private bool ContainsScreenPoint(int x, int y)
    {
        var size = FrameSize ?? ClientSize;
        var right = Position.X + size.Width * DesktopScaling;
        var bottom = Position.Y + size.Height * DesktopScaling;

        return x >= Position.X && x < right && y >= Position.Y && y < bottom;
    }
}
