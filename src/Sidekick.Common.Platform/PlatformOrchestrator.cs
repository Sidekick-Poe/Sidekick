using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.Versioning;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Platform.Linux;
using Sidekick.Common.Platform.Linux.Dependencies;
using Sidekick.Common.Platform.Linux.Wayland;
using WaylandClipboard = Sidekick.Common.Platform.Linux.Wayland.WaylandClipboard;

namespace Sidekick.Common.Platform;

/// <summary>
/// Centralizes platform detection, initialization, and service registration.
/// Determines the operating system and session type (X11/Wayland/KDE),
/// and selects the appropriate platform-specific implementations.
/// </summary>
public static class PlatformOrchestrator
{
    /// <summary>
    /// Gets a value indicating whether the current platform is Windows.
    /// </summary>
    public static bool IsWindows => OperatingSystem.IsWindows();

    /// <summary>
    /// Gets a value indicating whether the current platform is Linux.
    /// </summary>
    [SupportedOSPlatformGuard(PlatformNames.Linux)]
    public static bool IsLinux => OperatingSystem.IsLinux();

    /// <summary>
    /// Gets a value indicating whether the Wayland clipboard (wl-clipboard) should be used.
    /// True only on KDE Wayland when wl-clipboard is installed.
    /// </summary>
    private static bool UseWaylandClipboard
    {
        get
        {
            if (!IsLinux) return false;
            if (!WaylandSession.IsKde) return false;
            return WaylandClipboard.IsInstalled;
        }
    }

    /// <summary>
    /// Performs platform-specific initialization before the application starts.
    /// On Linux, this sets environment variables for native libraries and initializes the Wayland session.
    /// Call this once at the very beginning of <c>Program.Main</c>, before building the app.
    /// </summary>
    public static void Initialize()
    {
        if (!IsLinux) return;

        // WebKitGTK's DMA-BUF renderer leaves every window blank on some drivers (NVIDIA among them).
        // A value the user set themselves is kept.
        Linux.X11.Libc.setenv("WEBKIT_DISABLE_DMABUF_RENDERER", "1", 0);

        WaylandSession.Initialize();
    }

    /// <summary>
    /// Registers all platform-specific services in the DI container.
    /// Delegates to the orchestrator to select the correct implementations
    /// based on the detected platform and session type.
    /// </summary>
    /// <param name="services">The service collection to register services in.</param>
    /// <returns>The service collection with platform services registered.</returns>
    public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services)
    {
        services.TryAddTransient<IClipboardProvider, ClipboardProvider>();
        RegisterClipboardService(services);

        services.TryAddSingleton<IInterprocessService, InterprocessService>();
        RegisterProcessProvider(services);
        RegisterHotkeyBlocker(services);

        services.AddSidekickInitializableService<IInputProvider, InputProvider>();
        services.TryAddSingleton<IGameLogProvider, GameLogProvider>();

        RegisterLinuxDependencies(services);

        return services;
    }

    private static void RegisterClipboardService(IServiceCollection services)
    {
        if (UseWaylandClipboard)
        {
            services.TryAddTransient<ISystemClipboard, WaylandClipboard>();
        }
        else
        {
            services.TryAddTransient<ISystemClipboard, TextCopyClipboard>();
        }
    }

    private static void RegisterProcessProvider(IServiceCollection services)
    {
        if (OperatingSystem.IsWindows())
        {
            services.AddSidekickInitializableService<IProcessProvider, Windows.Processes.WindowsProcessProvider>();
        }
        else if (OperatingSystem.IsLinux())
        {
            services.AddSidekickInitializableService<IProcessProvider, Linux.LinuxProcessProvider>();
        }
    }

    private static void RegisterHotkeyBlocker(IServiceCollection services)
    {
        if (OperatingSystem.IsLinux())
        {
            services.TryAddSingleton<Linux.X11.X11Connection>();
            services.AddSidekickInitializableService<IHotkeyBlocker, Linux.LinuxHotkeyBlocker>();
        }
        else if (OperatingSystem.IsWindows())
        {
            services.AddSidekickInitializableService<IHotkeyBlocker, Windows.WindowsHotkeyBlocker>();
        }
    }

    private static void RegisterLinuxDependencies(IServiceCollection services)
    {
        if (!OperatingSystem.IsLinux()) return;

        services.AddSingleton<IDependency, Linux.Dependencies.XselDependency>();
        services.AddSingleton<IDependency, Linux.Dependencies.WebKitGtkDependency>();
        services.AddSingleton<LinuxDependencyChecker>();
    }
}
