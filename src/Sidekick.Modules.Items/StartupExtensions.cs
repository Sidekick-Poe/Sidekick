using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Items.Keybinds;
using Sidekick.Modules.Items.Localization;
using Sidekick.Modules.Items.Trade;
namespace Sidekick.Modules.Items;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickItems(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddTransient<TradeResources>();
        services.AddScoped<TradeService>();

        services.AddSidekickInputHandler<PriceCheckItemKeybindHandler>();

        return services;
    }
}
