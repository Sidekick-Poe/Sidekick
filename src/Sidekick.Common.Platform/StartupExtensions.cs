using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Platform.Linux;
using Sidekick.Common.Platform.Linux.X11;

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
        }
        else if (OperatingSystem.IsLinux())
        {
            services.TryAddSingleton<X11Session>();
            services.TryAddSingleton<IHotkeyBlocker>(GetX11Session);
            services.AddSidekickInitializableService<IProcessProvider, ProcessProvider>();
        }

        services.TryAddSingleton<IHotkeyBlocker, NoHotkeyBlocker>();
        services.AddSidekickInitializableService<IInputProvider, InputProvider>();
        services.TryAddSingleton<IGameLogProvider, GameLogProvider>();

        return services;
    }

    /// <summary>
    /// The process provider and the hotkey blocker share the same X11 session.
    /// </summary>
    [SupportedOSPlatform(PlatformNames.Linux)]
    private static X11Session GetX11Session(IServiceProvider provider) => provider.GetRequiredService<X11Session>();
}
