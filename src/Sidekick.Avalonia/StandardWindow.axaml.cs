using System.Text.Json;
using Sidekick.Avalonia.Controls;

namespace Sidekick.Avalonia;

public partial class StandardWindow : BaseWindow
{
    private const int WIDTH = 768;
    private const int HEIGHT = 600;

    public StandardWindow() : base(HEIGHT, WIDTH, "StandardWindow")
    {
        InitializeComponent();
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
                await NormalizeView();
            }
        });
    }
}
