using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Blazor;
using Sidekick.Common.Platform;
using Sidekick.Modules.Maps.Keybinds;
using Sidekick.Modules.Maps.Localization;

namespace Sidekick.Modules.Maps
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickMaps(this IServiceCollection services)
        {
            services.AddSidekickModule(new SidekickModule()
            {
                Assembly = typeof(StartupExtensions).Assembly
            });

            services.AddTransient<MapInfoResources>();

            services.AddSidekickKeybind<OpenMapInfoKeybindHandler>();

            return services;
        }
    }
}
