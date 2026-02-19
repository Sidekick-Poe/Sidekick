using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Trade;

namespace Sidekick.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickData(
        this IServiceCollection services)
    {
        services.TryAddSingleton<DataProvider>();
        services.TryAddSingleton<TradeDataProvider>();
        services.TryAddSingleton<IFuzzyService, FuzzyService>();
        services.AddSidekickInitializableService<TradeInvariantStatProvider>();

        return services;
    }
}