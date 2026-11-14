using System.Text.Json;
using Avalonia.Controls;
using Sidekick.Avalonia.Controls;

namespace Sidekick.Avalonia;

public partial class StandardWindow : BaseWindow
{
    private const int WIDTH = 768;
    private const int HEIGHT = 600;

    private bool isLoaded;

    public StandardWindow() : base(HEIGHT, WIDTH, "StandardWindow")
    {
        InitializeComponent();
    }

    public async Task OpenView(string url)
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            // Only the first open loads the page. Closing the window only hides it, so the page stays loaded.
            if (!isLoaded)
            {
                isLoaded = true;
                Show();
                WebView.Navigate(new Uri(url));
                await NormalizeView();
                return;
            }

            if (!IsVisible)
            {
                Show();
                await NormalizeView();
            }
            else if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            // Navigate in the app, even to the current page: Blazor keeps the page's state, and the navigation recovers from an error page.
            _ = WebView.InvokeScript($"sidekick.app.navigationManager.navigateTo({JsonSerializer.Serialize(url)});");

            Activate();
        });
    }
}
