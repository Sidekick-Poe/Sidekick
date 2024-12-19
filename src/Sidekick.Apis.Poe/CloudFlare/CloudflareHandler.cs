using System.Net;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.CloudFlare;

public class CloudflareHandler
(
    ILogger<CloudflareHandler> logger,
    ISettingsService settingsService,
    ICloudflareService cloudflareService
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // First try with existing cookies
        await AddCookieToRequest(request);
        var response = await base.SendAsync(request, cancellationToken);

        // If we don't get a 403, there is nothing else to do in this handler
        if (response.StatusCode != HttpStatusCode.Forbidden)
        {
            return response;
        }

        logger.LogInformation("[CloudflareHandler] Received 403 response, attempting to handle Cloudflare challenge");

        // Show WebView2 window and wait for challenge completion
        var success = await cloudflareService.StartCaptchaChallenge(request.RequestUri!, cancellationToken);
        if (!success)
        {
            logger.LogWarning("[CloudflareHandler] Failed to complete Cloudflare challenge");
            return response;
        }

        // Retry the request with new cookies
        await AddCookieToRequest(request);
        var retryResponse = await base.SendAsync(request, cancellationToken);
        if (retryResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("[CloudflareHandler] Successfully completed Cloudflare challenge and retried request");
        }
        else
        {
            logger.LogWarning("[CloudflareHandler] Request still failed after completing Cloudflare challenge: {StatusCode}, {RequestHeaders}", retryResponse.StatusCode, request.Headers.ToString());
        }

        return retryResponse;
    }

    private async Task AddCookieToRequest(HttpRequestMessage request)
    {
        var cookie = await settingsService.GetString(SettingKeys.CloudflareCookie);
        if (!string.IsNullOrEmpty(cookie))
        {
            logger.LogInformation("[CloudflareHandler] Adding cookie to request");
            // Append the cookie to the `Cookie` header
            if (!request.Headers.Contains("Cookie"))
            {
                request.Headers.Add("Cookie", $"cf_clearance={cookie}");
            }
            else
            {
                request.Headers.Remove("Cookie");
                request.Headers.Add("Cookie", $"cf_clearance={cookie}");
            }
        }
    }
}
