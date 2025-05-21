using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Keyboards;
using Sidekick.Common.Platform.Localization;

namespace Sidekick.Common.Platform;

/// <summary>
/// Functions for startup configuration for platform related features
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds platform (operating system) functions to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add services to.</param>
    /// <param name="options">The platform options.</param>
    /// <returns>The service collection with services added.</returns>
    public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services, Action<PlatformOptions> options)
    {
        services.Configure(options);

        services.AddTransient<PlatformResources>();
        services.AddTransient<IClipboardProvider, ClipboardProvider>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSidekickInitializableService<IProcessProvider, Windows.Processes.ProcessProvider>();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSidekickInitializableService<IProcessProvider, Linux.Processes.ProcessProvider>();
        }

        services.AddSidekickInitializableService<IKeyboardProvider, KeyboardProvider>();
        services.AddSingleton<IGameLogProvider, GameLogProvider>();

        foreach (var inputHandler in SidekickConfiguration.InputHandlers)
        {
            SidekickConfiguration.InitializableServices.Add(inputHandler);
            services.AddSingleton(inputHandler);
        }

        return services;
    }
}
