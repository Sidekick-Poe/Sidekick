using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Common.Settings;
using Sidekick.Modules.Items.Keybinds;
using Sidekick.Modules.Items.Trade;
using Sidekick.Modules.Items.Trade.Localization;
namespace Sidekick.Modules.Items;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickItems(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddTransient<TradeResources>();
        services.AddScoped<TradeService>();

        services.AddSidekickInputHandler<PriceCheckItemKeybindHandler>();

        services.SetSidekickDefaultSetting(SettingKeys.KeyOpenPriceCheck, "Ctrl+D");
        services.SetSidekickDefaultSetting(SettingKeys.PriceCheckPredictionEnabled, true);

        return services;
    }
}
