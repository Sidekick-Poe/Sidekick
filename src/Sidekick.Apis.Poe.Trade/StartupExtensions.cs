using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Apis.Poe.Trade.ApiStatic;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.ApiStats.Fuzzy;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Leagues;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.ApiInformation;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade.Bulk;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items;
using Sidekick.Common;

namespace Sidekick.Apis.Poe.Trade;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeTradeApi(this IServiceCollection services)
    {
        services.AddTransient<TradeApiHandler>();

        services.AddHttpClient(TradeApiClient.ClientName)
            .AddHttpMessageHandler<TradeApiHandler>();

        services.AddTransient<ITradeApiClient, TradeApiClient>();
        services.AddTransient<PoeResources>();

        services.AddSingleton<IItemTradeService, ItemTradeService>();
        services.AddSingleton<ILeagueProvider, LeagueProvider>();
        services.AddSingleton<IBulkTradeService, BulkTradeService>();
        services.AddSingleton<IFuzzyService, FuzzyService>();
        services.AddSingleton<IApiInformationParser, ApiInformationParser>();

        services.AddSidekickInitializableService<IItemParser, ItemParser>();
        services.AddSidekickInitializableService<IPropertyParser, PropertyParser>();
        services.AddSidekickInitializableService<IApiItemProvider, ApiItemProvider>();
        services.AddSidekickInitializableService<IApiStaticDataProvider, ApiStaticDataProvider>();
        services.AddSidekickInitializableService<IInvariantStatsProvider, InvariantStatsProvider>();
        services.AddSidekickInitializableService<IApiStatsProvider, ApiStatsProvider>();
        services.AddSidekickInitializableService<IStatParser, StatParser>();
        services.AddSidekickInitializableService<IPseudoParser, PseudoParser>();
        services.AddSidekickInitializableService<ITradeFilterProvider, TradeFilterProvider>();

        return services;
    }
}
