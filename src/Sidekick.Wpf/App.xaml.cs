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
    private readonly IInterprocessService interprocessService;

    public App()
    {
        logger = Program.ServiceProvider.GetRequiredService<ILogger<App>>();
        interprocessService = Program.ServiceProvider.GetRequiredService<IInterprocessService>();

        DisableWindowsTheme();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _ = HandleInterprocessCommunications(e);

        AttachErrorHandlers();

        var cloudFlareHandler = Program.ServiceProvider.GetRequiredService<WpfCloudflareHandler>();
        cloudFlareHandler.Initialize();

        interprocessService.StartReceiving();

        var viewLocator = Program.ServiceProvider.GetRequiredService<IViewLocator>();
        viewLocator.Open(SidekickViewType.Standard, "/");
    }

    private async Task HandleInterprocessCommunications(StartupEventArgs e)
    {
        if (HasApplicationStartedUsingSidekickProtocol(e) && interprocessService.IsAlreadyRunning())
        {
            // If we reach here, that means the application was started using a sidekick:// link. We send a message to the already running instance in this case and close this new instance after.
            try
            {
                await interprocessService.SendMessage(e.Args[0]);
            }
            finally
            {
                logger.LogDebug("[Startup] Application is shutting down due to another instance running.");
                ShutdownAndExit();
            }
        }

        // Wait a second before starting to listen to interprocess communications.
        // This is necessary as when we are restarting as admin, the old non-admin instance is still running for a fraction of a second.
        await Task.Delay(2000);
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

    private static bool HasApplicationStartedUsingSidekickProtocol(StartupEventArgs e)
    {
        return e.Args.Length > 0 && e.Args[0].ToUpper().StartsWith("SIDEKICK://");
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
            LogException((Exception)e.ExceptionObject);
        };

        DispatcherUnhandledException += (_, e) =>
        {
            LogException(e.Exception);
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            LogException(e.Exception);
        };
    }

    private static void DisableWindowsTheme()
    {
        // Disable Aero theme text rendering
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
    }

    private void LogException(Exception ex)
    {
        logger.LogCritical(ex, "Unhandled exception.");
    }
}
