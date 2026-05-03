using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Folder;
using Sidekick.Common.Initialization;
using Sidekick.Common.Localization;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Common;

public static class ServiceCollectionExtensions
{
    private static SidekickApplicationType _applicationType = SidekickApplicationType.Unknown;

    public static IServiceCollection AddSidekickCommon(
        this IServiceCollection services,
        SidekickApplicationType applicationType)
    {
        _applicationType = applicationType;

        services.TryAddSingleton<IBrowserProvider, BrowserProvider>();
        services.TryAddSingleton<ICacheProvider, CacheProvider>();
        services.TryAddSingleton<IFolderProvider, FolderProvider>();
        services.TryAddSingleton<ISettingsService, SettingsService>();

        services.AddSidekickInitializableService<IUiLanguageProvider, UiLanguageProvider>();

        services.SetSidekickDefaultSetting(SettingKeys.LanguageParser, "en");
        services.SetSidekickDefaultSetting(SettingKeys.LanguageUi, "en");
        services.SetSidekickDefaultSetting(SettingKeys.Zoom, "1");
        services.SetSidekickDefaultSetting(SettingKeys.RetainClipboard, true);
        services.SetSidekickDefaultSetting(SettingKeys.UseHardwareAcceleration, true);

        services.Configure<SidekickConfiguration>(configuration =>
        {
            configuration.ApplicationType = applicationType;
        });

        return services;
    }

    /// <summary>
    ///     Adds a sidekick module to the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly of the module.</param>
    public static void AddSidekickModule(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.Configure<SidekickConfiguration>(configuration => { configuration.Modules.Add(assembly); });
    }

    /// <summary>
    ///     Adds an initializable service to the application.
    /// </summary>
    /// <param name="services">The service collection to add an initializable service.</param>
    /// <typeparam name="TService">The type of service to add to the application.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service.</typeparam>
    /// <returns>The service collection.</returns>
    public static void AddSidekickInitializableService<TService, TImplementation>(
        this IServiceCollection services)
        where TService : class, IInitializableService
        where TImplementation : class, TService
    {
        services.TryAddSingleton<TService, TImplementation>();
        services.Configure<SidekickConfiguration>(configuration =>
        {
            configuration.InitializableServices.Add(typeof(TService));
        });
    }

    public static void AddSidekickInitializableService<TService>(
        this IServiceCollection services)
        where TService : class, IInitializableService
    {
        services.TryAddSingleton<TService>();
        services.Configure<SidekickConfiguration>(configuration =>
        {
            configuration.InitializableServices.Add(typeof(TService));
        });
    }

    /// <summary>
    ///     Adds an input handler to the application
    /// </summary>
    /// <typeparam name="TInputHandler">The type of the input handler.</typeparam>
    /// <param name="services">The service collection to add the input handler to</param>
    public static void AddSidekickInputHandler<TInputHandler>(this IServiceCollection services)
        where TInputHandler : class, IInputHandler
    {
        services.Configure<SidekickConfiguration>(configuration =>
        {
            if (!configuration.ApplicationType.SupportsKeybinds()) return;

            configuration.InitializableServices.Add(typeof(TInputHandler));
            configuration.InputHandlers.Add(typeof(TInputHandler));
        });

        if (_applicationType.SupportsKeybinds())
        {
            services.TryAddSingleton<TInputHandler>();
        }
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