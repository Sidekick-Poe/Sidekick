using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Platform.Interprocess;

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
    /// <returns>The service collection with services added.</returns>
    public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services)
    {
        services.TryAddTransient<IClipboardProvider, ClipboardProvider>();
        services.TryAddSingleton<IInterprocessService, InterprocessService>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSidekickInitializableService<IProcessProvider, Windows.Processes.ProcessProvider>();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSidekickInitializableService<IProcessProvider, Linux.Processes.ProcessProvider>();
        }

        services.AddSidekickInitializableService<IInputProvider, InputProvider>();
        services.TryAddSingleton<IGameLogProvider, GameLogProvider>();

        return services;
    }
}
