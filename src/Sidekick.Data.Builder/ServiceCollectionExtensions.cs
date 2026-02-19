using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Data.Builder.Game;
using Sidekick.Data.Builder.Ninja;
using Sidekick.Data.Builder.Trade;

namespace Sidekick.Data.Builder;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickDataBuilder(
        this IServiceCollection services)
    {
        services.TryAddSingleton<NinjaDownloader>();
        services.TryAddSingleton<TradeDownloader>();
        services.TryAddSingleton<TradeStatBuilder>();
        services.TryAddSingleton<RepoeDownloader>();
        services.TryAddSingleton<DataBuilder>();

        return services;
    }
}