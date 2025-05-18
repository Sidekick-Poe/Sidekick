using Sidekick.Apis.Poe.Clients.Cloudflare;
using Sidekick.Apis.Poe.Clients.Limiter;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common.Exceptions;

namespace Sidekick.Apis.Poe.Clients;

public class PoeTradeHandler
(
    ICloudflareService cloudflareService,
    PoeApiHandlerService handlerService,
    ApiLimiterProvider limitProvider
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        await cloudflareService.InitializeHttpRequest(request);

        var limitHandler = limitProvider.Get(ClientNames.TradeClient);
        using var lease = await limitHandler.Lease(cancellationToken: cancellationToken);

        var response = await base.SendAsync(request, cancellationToken);
        response = await handlerService.HandleRedirect(base.SendAsync, request, response, cancellationToken);
        response = await handlerService.HandleForbidden(base.SendAsync, request, response, cancellationToken);

        await handlerService.LogRequest(request, response, cancellationToken);
        await handlerService.HandleTooManyRequests(response, cancellationToken);
        await handlerService.HandleUnauthorized(response, cancellationToken);
        await handlerService.HandleBadRequest(response, cancellationToken);

        await limitHandler.HandleResponse(response, cancellationToken);

        if (response.IsSuccessStatusCode) return response;

        throw new ApiErrorException();
    }
}
