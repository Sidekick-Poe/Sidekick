using System.Net;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Common.Cloudflare;
using Sidekick.Apis.Common.Limiter;
using Sidekick.Apis.Common.States;
using Sidekick.Apis.Poe.Account.Authentication;
using Sidekick.Common.Exceptions;

namespace Sidekick.Apis.Poe.Account.Clients;

public class PoeApiHandler
(
    ICloudflareService cloudflareService,
    IAuthenticationService authenticationService,
    IApiStateProvider apiStateProvider,
    ApiLimiterProvider limitProvider,
    ILogger<PoeApiHandler> logger
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        apiStateProvider.Update(PoeApiClient.ClientName, ApiState.Throttled);

        var limitHandler = limitProvider.Get(PoeApiClient.ClientName);
        using var lease = await limitHandler.Lease(cancellationToken: cancellationToken);
        apiStateProvider.Update(PoeApiClient.ClientName, ApiState.Working);

        request.Headers.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        await cloudflareService.InitializeHttpRequest(request);
        await authenticationService.InitializeHttpRequest(request);

        var response = await base.SendAsync(request, cancellationToken);
        response = await HandleRedirect(request, response, cancellationToken);
        response = await HandleForbidden(request, response, cancellationToken);

        await LogRequest(request, response, cancellationToken);
        HandleTooManyRequests(response);
        HandleUnauthorized(response);
        HandleBadRequest(response);

        await limitHandler.HandleResponse(response, cancellationToken);

        if (response.IsSuccessStatusCode) return response;

        throw new ApiErrorException();
    }

    private async Task<HttpResponseMessage> HandleRedirect(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != HttpStatusCode.Moved && response.StatusCode != HttpStatusCode.Redirect && response.StatusCode != HttpStatusCode.RedirectKeepVerb)
        {
            return response;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (responseContent.Contains("<center>cloudflare</center>"))
        {
            logger.LogWarning("[PoeApiHandler] Received a cloudflare redirect. Letting the handler continue.");
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
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> HandleForbidden(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
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

        var retryResponse = await base.SendAsync(request, cancellationToken);
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


    private async Task LogRequest(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken = default)
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

    private void HandleBadRequest(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.BadRequest) return;

        logger.LogWarning("[PoeApiHandler] BadRequest.");
    }

    private void HandleTooManyRequests(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.TooManyRequests) return;

        logger.LogWarning("[PoeApiHandler] TooManyRequests.");
        throw new SidekickException("Rate limit exceeded.", "The official trade website has a rate limit to avoid spam. Sidekick cannot change this.");
    }

    private void HandleUnauthorized(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized) return;

        logger.LogWarning("[PoeApiHandler] Unauthorized.");
        throw new SidekickException("Sidekick failed to communicate with the trade API.", "The trade website requires authentication, which Sidekick does not support currently.", "Try using a different game language and/or force to search using English only in the settings.");
    }
}
