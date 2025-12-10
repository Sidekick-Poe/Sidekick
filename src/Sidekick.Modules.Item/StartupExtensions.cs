using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Item.Keybinds;
using Sidekick.Modules.Item.Localization;
using Sidekick.Modules.Item.Trade;
namespace Sidekick.Modules.Item;

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
