using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Platform.Linux;
using Sidekick.Common.Platform.Linux.X11;
using Sidekick.Common.Platform.Windows;

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

        if (WaylandSession.IsKde && WaylandClipboard.IsInstalled)
        {
            services.TryAddTransient<ISystemClipboard, WaylandClipboard>();
        }
        else
        {
            services.TryAddTransient<ISystemClipboard, TextCopyClipboard>();
        }

        services.TryAddSingleton<IInterprocessService, InterprocessService>();

        if (OperatingSystem.IsWindows())
        {
            services.AddSidekickInitializableService<IProcessProvider, Windows.Processes.ProcessProvider>();
            services.AddSidekickInitializableService<IHotkeyBlocker, WindowsHotkeyBlocker>();
        }
        else if (OperatingSystem.IsLinux())
        {
            services.TryAddSingleton<X11Connection>();

            // The hotkey blocker follows the game focus through the same process provider instance.
            services.TryAddSingleton<ProcessProvider>();
            services.TryAddSingleton<IProcessProvider>(GetLinuxProcessProvider);
            services.AddSidekickInitializableService<IProcessProvider, ProcessProvider>();
            services.AddSidekickInitializableService<IHotkeyBlocker, LinuxHotkeyBlocker>();
        }

        services.AddSidekickInitializableService<IInputProvider, InputProvider>();
        services.TryAddSingleton<IGameLogProvider, GameLogProvider>();

        return services;
    }

    [SupportedOSPlatform(PlatformNames.Linux)]
    private static ProcessProvider GetLinuxProcessProvider(IServiceProvider provider) => provider.GetRequiredService<ProcessProvider>();
}
