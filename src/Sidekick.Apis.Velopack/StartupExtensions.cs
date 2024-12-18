using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.Velopack;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickVelopack(this IServiceCollection services)
    {
        services.AddTransient<IAutoUpdater, AutoUpdater>();

        return services;
    }
}
