using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Common.Settings;
using Sidekick.Modules.Wealth.Provider;

namespace Sidekick.Modules.Wealth;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSingleton<WealthProvider>();

        services.SetSidekickDefaultSetting(SettingKeys.WealthItemTotalMinimum, WealthSettings.WealthItemTotalMinimum);

        return services;
    }
}
