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
    private readonly SemaphoreSlim challengeSemaphore = new(1, 1);
    private bool isHandlingChallenge;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // TODO : Set cf cookie

        // First try with existing cookies
        var response = await base.SendAsync(request, cancellationToken);

        // If we get a 403 and it's not already handling a challenge, we might need to solve one
        if (response.StatusCode == HttpStatusCode.Forbidden && !isHandlingChallenge)
        {
            logger.LogInformation("[CloudflareHandler] Received 403 response, attempting to handle Cloudflare challenge");

            try
            {
                await challengeSemaphore.WaitAsync(cancellationToken);
                isHandlingChallenge = true;

                // Show WebView2 window and wait for challenge completion
                var success = await cloudflareService.StartCaptchaChallenge(request.RequestUri!, cancellationToken);
                if (!success)
                {
                    logger.LogWarning("[CloudflareHandler] Failed to complete Cloudflare challenge");
                    return response;
                }

                // Retry the request with new cookies
                var retryResponse = await base.SendAsync(request, cancellationToken);
                if (retryResponse.IsSuccessStatusCode)
                {
                    logger.LogInformation("[CloudflareHandler] Successfully completed Cloudflare challenge and retried request");
                    return retryResponse;
                }

                logger.LogWarning("[CloudflareHandler] Request still failed after completing Cloudflare challenge: {StatusCode}", retryResponse.StatusCode);
                return retryResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[CloudflareHandler] Error handling Cloudflare challenge");
                return response;
            }
            finally
            {
                isHandlingChallenge = false;
                challengeSemaphore.Release();
            }
        }

        return response;
    }
}
