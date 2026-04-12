using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.About;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickAbout(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        return services;
    }
}
