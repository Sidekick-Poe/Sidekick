using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Leagues;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade;
using Sidekick.Common;
using Sidekick.Data.Stats;

namespace Sidekick.Apis.Poe.Trade;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeTradeApi(this IServiceCollection services)
    {
        services.TryAddTransient<TradeApiHandler>();

        services.AddHttpClient(TradeApiClient.ClientName)
            .AddHttpMessageHandler<TradeApiHandler>();

        services.TryAddTransient<ITradeApiClient, TradeApiClient>();
        services.TryAddTransient<PoeResources>();

        services.TryAddSingleton<IItemTradeService, ItemTradeService>();
        services.TryAddSingleton<ILeagueProvider, LeagueProvider>();
        services.AddSidekickInitializableService<IItemDefinitionParser, ItemDefinitionParser>();

        services.AddSidekickInitializableService<IItemParser, ItemParser>();
        services.AddSidekickInitializableService<IPropertyParser, PropertyParser>();
        services.AddSidekickInitializableService<IStatParser, StatParser>();
        services.AddSidekickInitializableService<IPseudoParser, PseudoParser>();
        services.AddSidekickInitializableService<ITradeFilterProvider, TradeFilterProvider>();

        services.TryAddSingleton<CurrencyFilterFactory>();
        services.TryAddSingleton<PlayerStatusFilterFactory>();

        services.SetSidekickDefaultSetting(AutoSelectPreferences.DefaultNormalizeBySettingKey, 0.1);
        services.SetSidekickDefaultSetting(AutoSelectPreferences.DefaultFillMinSettingKey, true);
        services.SetSidekickDefaultSetting(AutoSelectPreferences.DefaultSelectCategoriesSettingKey, new List<StatCategory> { StatCategory.Fractured });

        return services;
    }
}
