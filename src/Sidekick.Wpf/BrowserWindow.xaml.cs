using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Sidekick.Common.Dialogs;
using Sidekick.Wpf.Helpers;
using Application=System.Windows.Application;

namespace Sidekick.Wpf;

public partial class BrowserWindow
{
    private readonly ILogger logger;
    private readonly BrowserDialogProvider.OpenedArgs args;
    private string? userAgent;

    public BrowserWindow(ILogger logger, BrowserDialogProvider.OpenedArgs args)
    {
        this.logger = logger;
        this.args = args;
        InitializeComponent();

        _ = Application.Current.Dispatcher.Invoke(async () =>
        {
            Topmost = true;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.NoResize;

            await WebView.EnsureCoreWebView2Async();

            // Get the actual user agent the browser uses and save it.
            userAgent = await WebView.CoreWebView2.ExecuteScriptAsync("navigator.userAgent");
            userAgent = userAgent.Trim('\"');

            // Handle cookie changes by checking cookies after navigation
            WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            WebView.Source = args.Uri;

            // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
            WebView.Visibility = Visibility.Visible;

            // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set a background color. Otherwise, mouse clicks will go through the window.
            Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
            Opacity = 0.01;

            CenterHelper.Center(this);
            Activate();
        });
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        logger.LogInformation("[BrowserWindow] Closing the window.");
        if (!args.TaskCompletion.Task.IsCompleted)
        {
            args.TaskCompletion.SetResult(new BrowserDialogProvider.Result(
                                             Uri: WebView.Source,
                                             Success: false,
                                             UserAgent: userAgent,
                                             Cookies: new Dictionary<string, string>(),
                                             JsonContent: null));
        }

        UnregisterName("Grid");
        UnregisterName("WebView");

        base.OnClosing(e);
    }

    private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        try
        {
            logger.LogInformation("[BrowserWindow] Navigation completed at " + WebUtility.UrlDecode(WebView.Source?.ToString()));

            var cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync(args.Uri.GetLeftPart(UriPartial.Authority));
            var jsonContent = await GetPageJson();

            var options = new BrowserDialogProvider.ShouldCompleteArgs(
                Uri: WebView.Source,
                Cookies: cookies.ToDictionary(c => c.Name, c => c.Value),
                JsonContent: jsonContent.Content,
                IsJson: jsonContent.IsJson
            );
            if (!args.ShouldComplete.Invoke(options)) return;

            logger.LogInformation("[BrowserWindow] Completing the browser request.");
            args.TaskCompletion.SetResult(new BrowserDialogProvider.Result(
                                             Uri: WebView.Source,
                                             Success: true,
                                             UserAgent: userAgent,
                                             Cookies: cookies.ToDictionary(c => c.Name, c => c.Value),
                                             JsonContent: jsonContent.Content));
            Dispatcher.Invoke(Close);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[BrowserWindow] Error handling cookie check");
        }
    }

    private async Task<(bool IsJson, string Content)> GetPageJson()
    {
        try
        {
            // Inject JavaScript to check if the page content is JSON
            var script = @"
            (function() {
                try {
                    // Get the content of the body or response
                    let content = document.body.innerHTML;
                    if (document.body.childNodes.length > 0) {
                        if (document.body.childNodes[0].innerHTML) {
                            content = document.body.childNodes[0].innerHTML;
                        } else if (document.body.childNodes[0].data) {
                            content = document.body.childNodes[0].data;
                        }
                    }
                    
                    // Attempt to parse JSON
                    JSON.parse(content);
                    
                    // If successful, return the content
                    return content;
                } catch {
                    // If parsing fails, return null
                    return null;
                }
            })();";

            // Execute the script in the WebView's context
            var result = await WebView.CoreWebView2.ExecuteScriptAsync(script);
            if (string.IsNullOrEmpty(result))
            {
                logger.LogInformation("[BrowserWindow] The content is not JSON.");
                return (false, string.Empty);
            }

            logger.LogInformation("[BrowserWindow] The content is JSON.");
            return (true, result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[BrowserWindow] Error determining if content is JSON.");
            return (false, string.Empty);
        }
    }
}
