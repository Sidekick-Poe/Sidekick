using System.Text.Json;
using Sidekick.Avalonia.Controls;

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
}
