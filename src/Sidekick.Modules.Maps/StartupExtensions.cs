using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Maps.Keybinds;
using Sidekick.Modules.Maps.Localization;

namespace Sidekick.Modules.Maps;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickMaps(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddTransient<MapInfoResources>();

        services.AddSidekickKeybind<OpenMapInfoKeybindHandler>();

        return services;
    }
}
