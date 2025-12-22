using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Services;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly ILogger<App> logger;

    public App()
    {
        logger = Program.ServiceProvider.GetRequiredService<ILogger<App>>();

        DisableWindowsTheme();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _ = CheckIsAlreadyRunning();

        AttachErrorHandlers();
        Program.ServiceProvider.GetRequiredService<WpfBrowserWindowProvider>();
        var viewLocator = Program.ServiceProvider.GetRequiredService<IViewLocator>();
        viewLocator.Open(SidekickViewType.Standard, "/");
    }

    private async Task CheckIsAlreadyRunning()
    {
        // Wait a second before starting to listen to interprocess communications.
        // This is necessary as when we are restarting as admin, the old non-admin instance is still running for a fraction of a second.
        await Task.Delay(2000);
        var interprocessService = Program.ServiceProvider.GetRequiredService<IInterprocessService>();
        if (interprocessService.IsAlreadyRunning())
        {
            logger.LogDebug("[Startup] Application is already running.");
            var viewLocator = Program.ServiceProvider.GetRequiredService<IViewLocator>();
            viewLocator.Close(SidekickViewType.Standard);
            viewLocator.Close(SidekickViewType.Overlay);
            var sidekickDialogs = Program.ServiceProvider.GetRequiredService<ISidekickDialogs>();
            await sidekickDialogs.OpenOkModal("Another instance of Sidekick is already running. Make sure to close all instances of Sidekick inside the Task Manager.");
            logger.LogDebug("[Startup] Application is shutting down due to another instance running.");
            ShutdownAndExit();
        }
    }

    private static void ShutdownAndExit()
    {
        Current.Dispatcher.Invoke(() =>
        {
            Current.Shutdown();
        });
        Environment.Exit(0);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (Program.ServiceProvider != null!)
        {
            Program.ServiceProvider.Dispose();
        }

        base.OnExit(e);
    }

    private void AttachErrorHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            logger.LogCritical((Exception)e.ExceptionObject, "Unhandled exception.");
        };

        DispatcherUnhandledException += (_, e) =>
        {
            logger.LogCritical(e.Exception, "Unhandled exception.");
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            logger.LogCritical(e.Exception, "Unhandled exception.");
        };
    }

    private static void DisableWindowsTheme()
    {
        // Disable Aero theme text rendering
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
    }
}
