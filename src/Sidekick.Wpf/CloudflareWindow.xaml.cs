using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.CloudFlare;
using Sidekick.Wpf.Helpers;
using Application=System.Windows.Application;

namespace Sidekick.Wpf;

public partial class CloudflareWindow
{
    private readonly ILogger logger;
    private readonly ICloudflareService cloudflareService;
    private readonly Uri uri;
    private bool challengeCompleted;

    public CloudflareWindow(ILogger logger, ICloudflareService cloudflareService, Uri uri)
    {
        this.logger = logger;
        this.cloudflareService = cloudflareService;
        this.uri = uri;
        InitializeComponent();
        Ready();
    }

    public void Ready()
    {
        _ = Application.Current.Dispatcher.Invoke(async () =>
        {
            Topmost = true;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.NoResize;

            await WebView.EnsureCoreWebView2Async();

            // Get the actual user agent the browser uses and save it.
            var userAgent = await WebView.CoreWebView2.ExecuteScriptAsync("navigator.userAgent");
            userAgent = userAgent.Trim('\"');
            await cloudflareService.SetUserAgent(userAgent);

            WebView.CoreWebView2.CookieManager.DeleteAllCookies();

            // Handle cookie changes by checking cookies after navigation
            WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            WebView.Source = uri;

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
        if (!challengeCompleted)
        {
            logger.LogInformation("[CloudflareWindow] Closing the window without completing the challenge, marking as failed");
            _ = cloudflareService.CaptchaChallengeFailed();
        }

        UnregisterName("Grid");
        UnregisterName("WebView");

        base.OnClosing(e);
    }

    private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        try
        {
            logger.LogInformation("[CloudflareWindow] Checking for Cloudflare cookie at " + WebUtility.UrlDecode(WebView.Source?.ToString()));

            var cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync(uri.GetLeftPart(UriPartial.Authority));
            if (cookies.FirstOrDefault(c => c.Name == "cf_clearance") == null)
            {
                logger.LogInformation("[CloudflareWindow] Cookie not found");

                var isJsonContent = await CheckIfContentIsJson();
                if (!isJsonContent)
                {
                    logger.LogInformation("[CloudflareWindow] Content is not JSON, checking for Cloudflare challenge");
                    return;
                }
            }

            // Store the Cloudflare cookies
            challengeCompleted = true;
            _ = cloudflareService.CaptchaChallengeCompleted(cookies.ToDictionary(c => c.Name, c => c.Value));
            logger.LogInformation("[CloudflareWindow] Navigation completed, challenge likely completed");

            Dispatcher.Invoke(Close);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CloudflareWindow] Error handling cookie check");
        }
    }

    private async Task<bool> CheckIfContentIsJson()
    {
        try
        {
            // Inject JavaScript to check if the page content is JSON
            var script = @"
            (function() {
                try {
                    // Get the content of the body or response
                    const content = document.body.innerText || document.body.textContent;
                    
                    // Attempt to parse JSON
                    JSON.parse(content);
                    
                    // If successful, return true
                    return true;
                } catch {
                    // If parsing fails, return false
                    return false;
                }
            })();
        ";

            // Execute the script in the WebView's context
            var result = await WebView.CoreWebView2.ExecuteScriptAsync(script);

            // Handle the result
            if (bool.TryParse(result, out var isJson) && isJson)
            {
                logger.LogInformation("[CloudflareWindow] The content is JSON.");
                return true;
            }

            logger.LogInformation("[CloudflareWindow] The content is not JSON.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CloudflareWindow] Error determining if content is JSON.");
        }

        return false;
    }
}
