using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.Wealth;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSingleton<WealthProvider>();
        services.AddSingleton<WealthParserBkp>();
        // services.AddSidekickKeybind<OpenWealthKeybindHandler>();

        return services;
    }
}
