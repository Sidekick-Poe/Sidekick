using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Updater;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickUpdater(this IServiceCollection services)
    {
        services.AddTransient<IAutoUpdater, AutoUpdater>();
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        return services;
    }
}
