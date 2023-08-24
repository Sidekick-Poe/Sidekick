using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Trade.Keybinds;
using Sidekick.Modules.Trade.Localization;

namespace Sidekick.Modules.Trade
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickTrade(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddTransient<TradeResources>();
            services.AddTransient<PoeNinjaResources>();

            services.AddSidekickKeybind<PriceCheckItemKeybindHandler>();

            return services;
        }
    }
}
