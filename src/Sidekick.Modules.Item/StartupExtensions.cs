using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Trade.Keybinds;
using Sidekick.Modules.Trade.Localization;
using Sidekick.Modules.Trade.Trade;

namespace Sidekick.Modules.Trade;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickTrade(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddTransient<TradeResources>();
        services.AddScoped<TradeService>();

        services.AddSidekickInputHandler<PriceCheckItemKeybindHandler>();

        return services;
    }
}
