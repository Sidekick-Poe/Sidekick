using System.Text.Json;
using Sidekick.Avalonia.Controls;

namespace Sidekick.Avalonia;

public partial class SplashWindow : BaseWindow
{
    /// <summary>
    /// The default width, which fits the initialization and update check steps. Steps that need more room request a bigger size.
    /// </summary>
    private const int WIDTH = 480;

    /// <summary>
    /// The default height, which fits the initialization and update check steps. Steps that need more room request a bigger size.
    /// </summary>
    private const int HEIGHT = 320;

    public SplashWindow() :
        base(HEIGHT, WIDTH)
    {
        InitializeComponent();
    }

    /// <summary>
    /// The splash is only shown at launch and for updates, so it is released as soon as it is hidden.
    /// </summary>
    protected override TimeSpan? ReleaseDelay => TimeSpan.Zero;

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
}
