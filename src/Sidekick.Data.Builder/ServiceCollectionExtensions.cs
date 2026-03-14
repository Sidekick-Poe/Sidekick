using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Data.Builder.Leagues;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Pseudo;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Stats;
using Sidekick.Data.Builder.StatsInvariant;
using Sidekick.Data.Builder.Trade;

namespace Sidekick.Data.Builder;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickDataBuilder(
        this IServiceCollection services)
    {
        services.TryAddSingleton<LeagueBuilder>();

        services.TryAddSingleton<NinjaDownloader>();

        services.TryAddSingleton<PseudoBuilder>();

        services.TryAddSingleton<RepoeDownloader>();

        services.TryAddSingleton<StatBuilder>();

        services.TryAddSingleton<TradeDownloader>();
        services.TryAddSingleton<TradeStatProvider>();
        services.TryAddSingleton<StatsInvariantBuilder>();

        services.TryAddSingleton<DataBuilder>();

        return services;
    }
}