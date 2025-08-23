using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Wealth.Provider;

namespace Sidekick.Modules.Wealth;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSingleton<WealthProvider>();

        return services;
    }
}
