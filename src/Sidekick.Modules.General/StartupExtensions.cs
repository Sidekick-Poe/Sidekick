using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Common.Blazor.Initialization;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Modules.General.Keybinds;
using Sidekick.Modules.General.Settings;

namespace Sidekick.Modules.General;

/// <summary>
/// Startup configuration functions for the general module
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds the general module services to the service collection
    /// </summary>
    /// <param name="services">The services collection to add services to</param>
    /// <returns>The service collection with services added</returns>
    public static IServiceCollection AddSidekickGeneral(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSidekickInputHandler<CloseOverlayKeybindHandler>();
        services.AddSidekickInputHandler<CloseOverlayWithEscHandler>();
        services.AddSidekickInputHandler<FindItemKeybindHandler>();
        services.AddSidekickInputHandler<OpenWikiPageKeybindHandler>();
        services.AddSidekickInputHandler<OpenInCraftOfExileHandler>();
        services.AddSidekickInputHandler<MouseWheelHandler>();

        services.AddTransient<InitializationResources>();
        services.AddTransient<SettingsResources>();

        services.SetSidekickDefaultSetting(SettingKeys.KeyClose, "Space");
        services.SetSidekickDefaultSetting(SettingKeys.KeyOpenWiki, "Alt+W");
        services.SetSidekickDefaultSetting(SettingKeys.KeyFindItems, "Ctrl+F");
        services.SetSidekickDefaultSetting(SettingKeys.MouseWheelNavigateStash, true);
        services.SetSidekickDefaultSetting(SettingKeys.EscapeClosesOverlays, true);
        services.SetSidekickDefaultSetting(SettingKeys.OpenHomeOnLaunch, true);
        services.SetSidekickDefaultSetting(SettingKeys.PreferredWiki, WikiSetting.PoeWiki.GetValueAttribute());

        return services;
    }
}
