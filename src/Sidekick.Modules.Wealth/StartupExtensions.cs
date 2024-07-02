using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Wealth.Keybinds;

namespace Sidekick.Modules.Wealth
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddSingleton<WealthParser>();
            services.AddSidekickKeybind<OpenWealthKeybindHandler>();

            return services;
        }
    }
}
