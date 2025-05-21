using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.RegexHotkeys.Keybinds;

namespace Sidekick.Modules.RegexHotkeys;

/// <summary>
/// Startup configuration functions for the regex hotkeys module
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds the regex hotkeys module services to the service collection
    /// </summary>
    /// <param name="services">The services collection to add services to</param>
    /// <returns>The service collection with services added</returns>
    public static IServiceCollection AddSidekickRegexHotkeys(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddTransient<RegexHotkeysResources>();

        services.AddSidekickInputHandler<RegexHotkeyHandler>();

        return services;
    }
}
