using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Common.Cloudflare;
using Sidekick.Apis.Common.Limiter;
using Sidekick.Apis.Common.States;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Clients;

public class TradeApiHandler
(
    ICloudflareService cloudflareService,
    ApiLimiterProvider limitProvider,
    IApiStateProvider apiStateProvider,
    IGameLanguageProvider gameLanguageProvider,
    ILogger<TradeApiHandler> logger
) : DelegatingHandler
{
    private JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        await cloudflareService.InitializeHttpRequest(TradeApiClient.ClientName, request, cancellationToken);

        apiStateProvider.Update(TradeApiClient.ClientName, ApiState.Throttled);

        var limitHandler = limitProvider.Get(TradeApiClient.ClientName);
        using var lease = await limitHandler.Lease(cancellationToken: cancellationToken);
        apiStateProvider.Update(TradeApiClient.ClientName, ApiState.Working);

        var response = await base.SendAsync(request, cancellationToken);
        response = await HandleRedirect(request, response, cancellationToken);
        response = await HandleForbidden(request, response, cancellationToken);

        await LogRequest(request, response, cancellationToken);
        await HandleTooManyRequests(response, cancellationToken);
        HandleUnauthorized(response);
        await HandleBadRequest(response, cancellationToken);

        limitHandler.HandleResponse(response, cancellationToken);

        if (response.IsSuccessStatusCode) return response;

        throw new ApiErrorException();
    }

    private async Task<HttpResponseMessage> HandleRedirect(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.Moved
            && response.StatusCode != HttpStatusCode.MovedPermanently
            && response.StatusCode != HttpStatusCode.Redirect
            && response.StatusCode != HttpStatusCode.RedirectKeepVerb)
        {
            return response;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (responseContent.Contains("<center>cloudflare</center>"))
        {
            var isChinese = gameLanguageProvider.IsChinese();
            if (isChinese)
            {
                logger.LogWarning("[PoeTradeHandler] Invalid chinese settings. Throwing exception.");
                throw new SidekickException("Sidekick failed to communicate with the trade API.", "The trade website requires authentication, which Sidekick does not support currently.", "Try using a different game language in the settings.");
            }

            logger.LogWarning("[PoeTradeHandler] Received a cloudflare redirect. Letting the handler continue.");
        }

        // Get redirect URL from the "Location" header
        var redirectUri = response.Headers.Location;
        logger.LogInformation("[PoeTradeHandler] Redirection status code detected.");
        if (redirectUri == null)
        {
            return response;
        }

        logger.LogInformation("[PoeTradeHandler] Redirecting to {redirectUri}.", redirectUri);

        request.RequestUri = redirectUri;

        // Retry the request with the new URI
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> HandleForbidden(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.Forbidden) return response;

        logger.LogInformation("[PoeTradeHandler] Received 403 response, attempting to handle Cloudflare challenge");

        // Show WebView2 window and wait for challenge completion
        var success = await cloudflareService.Challenge(TradeApiClient.ClientName, request.RequestUri!, cancellationToken);
        if (!success)
        {
            logger.LogWarning("[PoeTradeHandler] Failed to complete Cloudflare challenge");
            return response;
        }

        // Retry the request with new cookies
        await cloudflareService.InitializeHttpRequest(TradeApiClient.ClientName, request, cancellationToken);

        var retryResponse = await base.SendAsync(request, cancellationToken);
        if (retryResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("[PoeTradeHandler] Successfully completed Cloudflare challenge and retried request");
        }
        else
        {
            logger.LogWarning("[PoeTradeHandler] Request still failed after completing Cloudflare challenge: {StatusCode},\n{RequestHeaders}", retryResponse.StatusCode, request.Headers.ToString());
        }

        return retryResponse;
    }

    private async Task LogRequest(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode) return;

        logger.LogWarning("[PoeTradeHandler] Failed API call information");
        logger.LogWarning("[PoeTradeHandler] Uri: {uri}", request.RequestUri);

        if (request.Content != null)
        {
            var body = await request.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("[PoeTradeHandler] Body: {uri}", body);
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogWarning("[PoeTradeHandler] Response: {responseCode} {responseMessage}", response.StatusCode, content);
    }

    private async Task HandleBadRequest(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.BadRequest) return;

        logger.LogWarning("[PoeTradeHandler] BadRequest.");
        var apiError = await ParseErrorResponse(response, cancellationToken);
        if (apiError?.Error?.Message?.StartsWith("Query is too complex.") ?? false)
        {
            throw new SidekickException("Query is too complex.", "The official trade website has limit on complex queries. Sidekick cannot change this.", "Use the official website to search for your current item.");
        }
    }

    private async Task HandleTooManyRequests(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.StatusCode != HttpStatusCode.TooManyRequests) return;

        logger.LogWarning("[PoeTradeHandler] TooManyRequests.");
        var apiError = await ParseErrorResponse(response, cancellationToken);
        throw new SidekickException("Rate limit exceeded.", "The official trade website has a rate limit to avoid spam. Sidekick cannot change this.", apiError?.Error?.Message ?? string.Empty);
    }

    private void HandleUnauthorized(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized) return;

        logger.LogWarning("[PoeTradeHandler] Unauthorized.");
        throw new SidekickException("Sidekick failed to communicate with the trade API.", "The trade website requires authentication, which Sidekick does not support currently.", "Try using a different game language and/or force to search using English only in the settings.");
    }

    private async Task<ApiErrorResponse?> ParseErrorResponse(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<ApiErrorResponse>(content, JsonSerializerOptions, cancellationToken);
        }
        catch (Exception)
        {
            logger.LogWarning("[PoeTradeHandler] Failed to parse the error response.");
        }

        return null;
    }
}
