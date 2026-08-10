using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Settings.Languages;
using Sidekick.Common.Settings.Localization;
namespace Sidekick.Common.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonSettings(this IServiceCollection services)
    {
        services.TryAddSingleton<ISettingsService, SettingsService>();

        services.SetSidekickDefaultSetting(SettingKeys.LanguageParser, "en");
        services.SetSidekickDefaultSetting(SettingKeys.LanguageUi, "en");
        services.SetSidekickDefaultSetting(SettingKeys.Zoom, "1");
        services.SetSidekickDefaultSetting(SettingKeys.RetainClipboard, true);
        services.SetSidekickDefaultSetting(SettingKeys.UseHardwareAcceleration, true);

        services.AddSidekickInitializableService<ICurrentGameLanguage, CurrentGameLanguage>();
        services.AddSidekickInitializableService<IUiLanguageProvider, UiLanguageProvider>();

        return services;
    }

    public static void SetSidekickDefaultSetting(this IServiceCollection services,
        string key,
        object value)
    {
        services.Configure<SidekickConfiguration>(configuration =>
        {
            if (!configuration.DefaultSettings.TryAdd(key, value))
            {
                configuration.DefaultSettings[key] = value;
            }
        });
    }
}