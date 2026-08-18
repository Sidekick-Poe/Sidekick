using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Trade;

namespace Sidekick.Apis.Poe.Trade;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeTradeApi(this IServiceCollection services)
    {
        services.TryAddTransient<TradeApiHandler>();

        services.AddHttpClient(TradeApiClient.ClientName)
            .AddHttpMessageHandler<TradeApiHandler>();

        services.TryAddTransient<ITradeApiClient, TradeApiClient>();
        services.TryAddSingleton<IItemTradeService, ItemTradeService>();

        return services;
    }
}
