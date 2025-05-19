using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.General.Keybinds;

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
        services.AddSidekickKeybind<CloseOverlayKeybindHandler>();
        services.AddSidekickKeybind<CloseOverlayWithEscHandler>();
        services.AddSidekickKeybind<FindItemKeybindHandler>();
        services.AddSidekickKeybind<OpenWikiPageKeybindHandler>();
        services.AddSidekickKeybind<OpenInCraftOfExileHandler>();
        services.AddSidekickInitializableService<MouseWheelHandler, MouseWheelHandler>();

        return services;
    }
}
