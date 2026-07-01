using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Modules.Initialization.Localization;
namespace Sidekick.Modules.Initialization;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickInitialization(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.TryAddTransient<InitializationResources>();

        return services;
    }
}
