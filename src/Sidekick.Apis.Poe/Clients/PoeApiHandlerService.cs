using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Cloudflare;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Clients;

public class PoeApiHandlerService
(
    ILogger<PoeApiHandlerService> logger,
    ICloudflareService cloudflareService,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider
)
{
    private JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<HttpResponseMessage> HandleForbidden(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send, HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.Forbidden) return response;

        logger.LogInformation("[PoeApiHandler] Received 403 response, attempting to handle Cloudflare challenge");

        // Show WebView2 window and wait for challenge completion
        var success = await cloudflareService.StartCaptchaChallenge(request.RequestUri!, cancellationToken);
        if (!success)
        {
            logger.LogWarning("[PoeApiHandler] Failed to complete Cloudflare challenge");
            return response;
        }

        // Retry the request with new cookies
        await cloudflareService.InitializeHttpRequest(request);

        var retryResponse = await send(request, cancellationToken);
        if (retryResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("[PoeApiHandler] Successfully completed Cloudflare challenge and retried request");
        }
        else
        {
            logger.LogWarning("[PoeApiHandler] Request still failed after completing Cloudflare challenge: {StatusCode},\n{RequestHeaders}", retryResponse.StatusCode, request.Headers.ToString());
        }

        return retryResponse;
    }

    public async Task<HttpResponseMessage> HandleRedirect(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send, HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.Moved && response.StatusCode != HttpStatusCode.Redirect && response.StatusCode != HttpStatusCode.RedirectKeepVerb)
        {
            return response;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (responseContent.Contains("<center>cloudflare</center>"))
        {
            var useInvariantTradeResults = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
            var isChinese = gameLanguageProvider.IsChinese();
            if (isChinese && !useInvariantTradeResults)
            {
                logger.LogWarning("[PoeTradeHandler] Invalid chinese settings. Throwing exception.");
                throw new SidekickException("Sidekick failed to communicate with the trade API.", "The trade website requires authentication, which Sidekick does not support currently.", "Try using a different game language and/or force to search using English only in the settings.");
            }

            logger.LogWarning("[PoeTradeHandler] Received a cloudflare redirect. Letting the handler continue.");
        }

        // Get redirect URL from the "Location" header
        var redirectUri = response.Headers.Location;
        logger.LogInformation("[PoeApiHandler] Redirection status code detected.");
        if (redirectUri == null)
        {
            return response;
        }

        logger.LogInformation("[PoeApiHandler] Redirecting to {redirectUri}.", redirectUri);

        request.RequestUri = redirectUri;

        // Retry the request with the new URI
        return await send(request, cancellationToken);
    }

    public async Task LogRequest(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode) return;

        logger.LogWarning("[PoeApiHandler] Failed API call information");
        logger.LogWarning("[PoeApiHandler] Uri: {uri}", request.RequestUri);

        if (request.Content != null)
        {
            var body = await request.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("[PoeApiHandler] Body: {uri}", body);
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogWarning("[PoeApiHandler] Response: {responseCode} {responseMessage}", response.StatusCode, content);
    }

    public async Task HandleBadRequest(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.BadRequest) return;

        logger.LogWarning("[PoeApiHandler] BadRequest.");
        var apiError = await ParseErrorResponse(response, cancellationToken);
        if (apiError?.Error?.Message?.StartsWith("Query is too complex.") ?? false)
        {
            throw new SidekickException("Query is too complex.", "The official trade website has limit on complex queries. Sidekick cannot change this.", "Use the official website to search for your current item.");
        }
    }

    public async Task HandleTooManyRequests(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.StatusCode != HttpStatusCode.TooManyRequests) return;

        logger.LogWarning("[PoeApiHandler] TooManyRequests.");
        var apiError = await ParseErrorResponse(response, cancellationToken);
        throw new SidekickException("Rate limit exceeded.", "The official trade website has a rate limit to avoid spam. Sidekick cannot change this.", apiError?.Error?.Message ?? string.Empty);
    }

    public Task HandleUnauthorized(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized) return Task.CompletedTask;

        logger.LogWarning("[PoeApiHandler] Unauthorized.");
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
            logger.LogWarning("[PoeApiHandler] Failed to parse the error response.");
        }

        return null;
    }
}
