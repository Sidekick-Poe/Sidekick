using System.Net;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Sidekick.Common.Dialogs;

namespace Sidekick.Avalonia;

public partial class BrowserWindow : Window
{
    private const int WIDTH = 800;
    private const int HEIGHT = 600;

    private readonly ILogger logger;
    private readonly BrowserDialogProvider.OpenedArgs args;

    private string? userAgent;
    private bool resultSet;

    public BrowserWindow(ILogger logger, BrowserDialogProvider.OpenedArgs args)
    {
        this.logger = logger;
        this.args = args;

        Title = "Sidekick";
        Width = WIDTH;
        Height = HEIGHT;
        MinWidth = WIDTH;
        MinHeight = HEIGHT;
        Background = new SolidColorBrush(Color.FromRgb(12, 10, 10));
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        WindowDecorations = WindowDecorations.Full;
        Topmost = true;
        ShowInTaskbar = true;
        CanResize = false;

        InitializeComponent();

        WebView.NavigationCompleted += WebView_NavigationCompleted;

        Opened += BrowserWindow_Opened;
        Closed += BrowserWindow_Closed;
    }

    private void BrowserWindow_Opened(object? sender, EventArgs e)
    {
        _ = Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                WebView.Navigate(args.Uri);

                WebView.IsVisible = true;

                Activate();
                Focus();

                userAgent = await GetUserAgent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[BrowserWindow] Error opening the browser window.");

                SetResult(new BrowserDialogProvider.Result(
                    Uri: WebView.Source,
                    Success: false,
                    UserAgent: userAgent,
                    Cookies: new Dictionary<string, string>(),
                    JsonContent: null));

                Close();
            }
        }, DispatcherPriority.ApplicationIdle);
    }

    private void BrowserWindow_Closed(object? sender, EventArgs e)
    {
        logger.LogInformation("[BrowserWindow] Closing the window.");

        SetResult(new BrowserDialogProvider.Result(
            Uri: WebView.Source,
            Success: false,
            UserAgent: userAgent,
            Cookies: new Dictionary<string, string>(),
            JsonContent: null));
    }

    private async void WebView_NavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        try
        {
            logger.LogInformation("[BrowserWindow] Navigation completed at {uri}", WebUtility.UrlDecode(WebView.Source.ToString()));

            var cookies = await GetCookies();
            var jsonContent = await GetPageJson();

            var options = new BrowserDialogProvider.ShouldCompleteArgs(
                Uri: WebView.Source,
                Cookies: cookies,
                JsonContent: jsonContent.Content,
                IsJson: jsonContent.IsJson);

            if (!args.ShouldComplete.Invoke(options))
            {
                return;
            }

            logger.LogInformation("[BrowserWindow] Completing the browser request.");

            SetResult(new BrowserDialogProvider.Result(
                Uri: WebView.Source,
                Success: true,
                UserAgent: userAgent,
                Cookies: cookies,
                JsonContent: jsonContent.Content));

            Close();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[BrowserWindow] Error handling navigation completion.");
        }
    }

    private async Task<string?> GetUserAgent()
    {
        try
        {
            var result = await WebView.InvokeScript("navigator.userAgent");
            return result?.Trim('"');
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[BrowserWindow] Error getting user agent.");
            return null;
        }
    }

    private async Task<Dictionary<string, string>> GetCookies()
    {
        var cookieDictionary = new Dictionary<string, string>();
        var cookieManager = WebView.TryGetCookieManager();
        if (cookieManager != null)
        {
            var cookies = await cookieManager.GetCookiesAsync();

            foreach (var cookie in cookies)
            {
                cookieDictionary[cookie.Name] = cookie.Value;
            }
        }

        return cookieDictionary;
    }

    private async Task<(bool IsJson, string Content)> GetPageJson()
    {
        try
        {
            var result = await WebView.InvokeScript(@"
                (function() {
                    try {
                        let content = document.body.innerHTML;

                        if (document.body.childNodes.length > 0) {
                            if (document.body.childNodes[0].innerHTML) {
                                content = document.body.childNodes[0].innerHTML;
                            } else if (document.body.childNodes[0].data) {
                                content = document.body.childNodes[0].data;
                            }
                        }

                        JSON.parse(content);

                        return content;
                    } catch {
                        return null;
                    }
                })();
            ");

            if (string.IsNullOrEmpty(result) || result == "null")
            {
                logger.LogInformation("[BrowserWindow] The content is not JSON.");
                return (false, string.Empty);
            }

            logger.LogInformation("[BrowserWindow] The content is JSON.");
            return (true, result.Trim('"'));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[BrowserWindow] Error determining if content is JSON.");
            return (false, string.Empty);
        }
    }

    private void SetResult(BrowserDialogProvider.Result result)
    {
        if (resultSet)
        {
            return;
        }

        resultSet = true;
        args.TaskCompletion.TrySetResult(result);
    }
}