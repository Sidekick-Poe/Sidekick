using Microsoft.Extensions.DependencyInjection;
using Velopack;

namespace Sidekick.Apis.Velopack;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickVelopack(this IServiceCollection services)
    {
        VelopackApp.Build().Run();

        services.AddTransient<IAutoUpdater, AutoUpdater>();

        return services;
    }
}
