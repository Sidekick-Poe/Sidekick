using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.Data;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickModuleData(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        return services;
    }
}
