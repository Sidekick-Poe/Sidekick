using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.Wealth;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSingleton<WealthParser>();
        // services.AddSidekickKeybind<OpenWealthKeybindHandler>();

        return services;
    }
}
