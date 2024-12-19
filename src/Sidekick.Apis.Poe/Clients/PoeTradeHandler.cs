using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.CloudFlare;

namespace Sidekick.Apis.Poe.Clients;

public class PoeTradeHandler
(
    ILogger<PoeTradeHandler> logger,
    ICloudflareService cloudflareService
) : DelegatingHandler
{
    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US", 0.9));
        request.Headers.Connection.Add("keep-alive");
        request.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true };
        request.Headers.Host = request.RequestUri?.Host;
        request.Headers.UserAgent.ParseAdd(UserAgent);
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.TryAddWithoutValidation("X-Powered-By", "Sidekick");

        // First try with existing cookies
        await cloudflareService.AddCookieToRequest(request);
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
        await cloudflareService.AddCookieToRequest(request);
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

}
