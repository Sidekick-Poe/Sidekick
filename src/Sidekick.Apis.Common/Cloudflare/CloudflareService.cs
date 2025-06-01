using Microsoft.Extensions.Logging;
using Sidekick.Common.Browser;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Common.Cloudflare;

public class CloudflareService
(
    ISettingsService settingsService,
    IBrowserWindowProvider browserWindowProvider,
    ILogger<CloudflareService> logger
) : ICloudflareService
{
    private Dictionary<string, Task<bool>> PendingChallenges { get; } = [];

    public Task<bool> Challenge(string clientName, Uri? uri = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[CloudflareService] Starting Cloudflare challenge.");

        if (uri == null)
        {
            logger.LogInformation("[CloudflareService] No uri provided, skipping.");
            return Task.FromResult(false);
        }

        if (PendingChallenges.TryGetValue(clientName, out var challenge))
        {
            return challenge;
        }

        PendingChallenges.Add(clientName, OpenBrowser(clientName, uri, cancellationToken));
        return PendingChallenges[clientName];
    }

    private bool ShouldComplete(BrowserCompletionOptions options)
    {
        return options.Cookies.ContainsKey("cf_clearance");
    }

    private async Task<bool> OpenBrowser(string clientName, Uri uri, CancellationToken cancellationToken)
    {
        var result = await browserWindowProvider.OpenBrowserWindow(new BrowserRequest()
                                                                    {
                                                                        Uri = uri,
                                                                        ShouldComplete = ShouldComplete,
                                                                    });
        if (!result.Success)
        {
            logger.LogInformation("[CloudflareService] Cloudflare challenge failed.");
            return false;
        }

        logger.LogInformation("[CloudflareService] Setting user agent to: " + result.UserAgent);
        await settingsService.Set(SettingKeys.CloudflareUserAgent, result.UserAgent);

        await browserWindowProvider.SaveCookies(clientName, result, cancellationToken);

        logger.LogInformation("[CloudflareService] Cloudflare challenge completed.");
        return true;
    }

    public async Task InitializeHttpRequest(string clientName, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.UserAgent.Clear();

        var userAgent = await settingsService.GetString(SettingKeys.CloudflareUserAgent);
        request.Headers.UserAgent.ParseAdd(!string.IsNullOrEmpty(userAgent) ? userAgent : BrowserWindowProvider.DefaultUserAgent);

        var cookieString = await browserWindowProvider.GetCookieString(clientName, cancellationToken);
        if (!string.IsNullOrEmpty(cookieString))
        {
            logger.LogInformation("[CloudflareService] Adding cookie to request");
            // Append the cookie to the `Cookie` header
            if (!request.Headers.Contains("Cookie"))
            {
                request.Headers.Add("Cookie", cookieString);
            }
            else
            {
                request.Headers.Remove("Cookie");
                request.Headers.Add("Cookie", cookieString);
            }
        }
        else
        {
            logger.LogInformation("[CloudflareService] No cookies found");
        }
    }
}
