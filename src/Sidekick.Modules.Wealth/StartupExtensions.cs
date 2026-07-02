using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Common.Settings;
using Sidekick.Modules.Wealth.Provider;

namespace Sidekick.Modules.Wealth;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.TryAddSingleton<WealthProvider>();

        services.SetSidekickDefaultSetting(SettingKeys.WealthItemTotalMinimum, WealthSettings.WealthItemTotalMinimum);

        return services;
    }
}
