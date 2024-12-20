using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.CloudFlare;
using Sidekick.Common.Exceptions;

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

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        // Sidekick does not support authentication yet.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new SidekickException("Sidekick failed to communicate with the trade API.", "The trade website requires authentication, which Sidekick does not support currently.", "Try using a different game language and/or force to search using English only in the settings.");
        }

        // 403 probably means a cloudflare issue.
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            logger.LogInformation("[PoeTradeHandler] Received 403 response, attempting to handle Cloudflare challenge");

            // Show WebView2 window and wait for challenge completion
            var success = await cloudflareService.StartCaptchaChallenge(request.RequestUri!, cancellationToken);
            if (!success)
            {
                logger.LogWarning("[PoeTradeHandler] Failed to complete Cloudflare challenge");
                return response;
            }

            // Retry the request with new cookies
            await cloudflareService.AddCookieToRequest(request);
            var retryResponse = await base.SendAsync(request, cancellationToken);
            if (retryResponse.IsSuccessStatusCode)
            {
                logger.LogInformation("[PoeTradeHandler] Successfully completed Cloudflare challenge and retried request");
            }
            else
            {
                logger.LogWarning("[PoeTradeHandler] Request still failed after completing Cloudflare challenge: {StatusCode}, {RequestHeaders}", retryResponse.StatusCode, request.Headers.ToString());
            }

            return retryResponse;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        string? body = null;
        if (request.Content != null)
        {
            body = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        logger.LogWarning("[PoeTradeHandler] Query Failed: {responseCode} {responseMessage}", response.StatusCode, content);
        logger.LogWarning("[PoeTradeHandler] Uri: {uri}", request.RequestUri);
        logger.LogWarning("[PoeTradeHandler] Body: {uri}", body);
        throw new ApiErrorException();
    }
}
