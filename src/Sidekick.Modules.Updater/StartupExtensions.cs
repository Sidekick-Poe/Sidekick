using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
namespace Sidekick.Modules.Updater;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickUpdater(this IServiceCollection services)
    {
        services.TryAddTransient<IAutoUpdater, AutoUpdater>();
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        return services;
    }
}
