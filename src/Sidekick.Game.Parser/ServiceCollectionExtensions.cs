using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Leagues;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Common;
using Sidekick.Common.Settings;
using Sidekick.Game.Parser.Stats;

namespace Sidekick.Game.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickGameParser(
        this IServiceCollection services)
    {
        services.TryAddTransient<PoeResources>();

        services.TryAddSingleton<LeagueProvider>();
        services.TryAddSingleton<ItemClassParser>();
        services.TryAddSingleton<ItemDefinitionParser>();
        services.TryAddSingleton<TradeFilterParser>();

        services.AddSidekickInitializableService<ItemParser>();
        services.AddSidekickInitializableService<PropertyParser>();
        services.AddSidekickInitializableService<StatParser, StatParser>();
        services.AddSidekickInitializableService<PseudoParser, PseudoParser>();
        services.AddSidekickInitializableService<TradeFilterProvider>();
        services.AddSidekickInitializableService<TextParser>();

        services.TryAddSingleton<CurrencyFilterFactory>();
        services.TryAddSingleton<PlayerStatusFilterFactory>();

        services.SetSidekickDefaultSetting(AutoSelectPreferences.DefaultNormalizeBySettingKey, 0.1);
        services.SetSidekickDefaultSetting(AutoSelectPreferences.DefaultFillMinSettingKey, true);
        services.SetSidekickDefaultSetting(AutoSelectPreferences.DefaultSelectCategoriesSettingKey, new List<StatCategory> { StatCategory.Fractured });

        return services;
    }
}