using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
namespace Sidekick.Modules.Updater;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickUpdater(this IServiceCollection services)
    {
        services.AddTransient<IAutoUpdater, AutoUpdater>();
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        return services;
    }
}
