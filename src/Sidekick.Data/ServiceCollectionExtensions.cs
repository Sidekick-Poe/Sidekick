using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Ninja;
using Sidekick.Data.Trade;

namespace Sidekick.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickData(
        this IServiceCollection services)
    {
        services.TryAddSingleton<DataProvider>();
        services.TryAddSingleton<NinjaDataProvider>();
        services.TryAddSingleton<TradeDataProvider>();
        services.TryAddSingleton<IFuzzyService, FuzzyService>();

        return services;
    }
}