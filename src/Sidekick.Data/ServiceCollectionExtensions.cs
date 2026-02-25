using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Ninja;
using Sidekick.Data.Stats;
using Sidekick.Data.Trade;

namespace Sidekick.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickData(
        this IServiceCollection services)
    {
        services.TryAddSingleton<DataProvider>();

        services.AddSidekickInitializableService<ICurrentGameLanguage, CurrentGameLanguage>();
        services.AddSingleton<IGameLanguageProvider, GameLanguageProvider>();

        services.TryAddSingleton<NinjaDataProvider>();

        services.TryAddSingleton<StatDataProvider>();

        services.TryAddSingleton<TradeDataProvider>();

        services.TryAddSingleton<IFuzzyService, FuzzyService>();

        return services;
    }
}