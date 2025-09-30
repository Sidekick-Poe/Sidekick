using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Bulk;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Fuzzy;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Leagues;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Requirements;
using Sidekick.Apis.Poe.Trade.Static;
using Sidekick.Apis.Poe.Trade.Trade;
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

        services.AddSingleton<ITradeSearchService, TradeSearchService>();
        services.AddSingleton<ILeagueProvider, LeagueProvider>();
        services.AddSingleton<IBulkTradeService, BulkTradeService>();
        services.AddSingleton<IModifierParser, ModifierParser>();
        services.AddSingleton<IFuzzyService, FuzzyService>();

        services.AddSingleton<IRequirementsParser, RequirementsParser>();
        services.AddSidekickInitializableService<IItemParser, ItemParser>();
        services.AddSidekickInitializableService<IPropertyParser, PropertyParser>();
        services.AddSidekickInitializableService<IApiItemProvider, ApiItemProvider>();
        services.AddSidekickInitializableService<IApiStaticDataProvider, ApiStaticDataProvider>();
        services.AddSidekickInitializableService<IInvariantModifierProvider, InvariantModifierProvider>();
        services.AddSidekickInitializableService<IModifierProvider, ModifierProvider>();
        services.AddSidekickInitializableService<IPseudoParser, PseudoParser>();
        services.AddSidekickInitializableService<IFilterProvider, FilterProvider>();
        services.AddSidekickInitializableService<IInvariantFilterProvider, InvariantFilterProvider>();

        return services;
    }
}
