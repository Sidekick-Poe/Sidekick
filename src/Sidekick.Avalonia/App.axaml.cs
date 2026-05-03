using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia;

public partial class App : Application
{
    private ILogger<App>? logger;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            logger = Program.ServiceProvider.GetService<ILogger<App>>();
            AttachErrorHandlers();
            _ = CheckIsAlreadyRunning();

            // Initialize host-level services (tray icon, language change listener)
            var appService = Program.ServiceProvider.GetRequiredService<IApplicationService>();
            _ = appService.Initialize();

            var viewLocator = Program.ServiceProvider.GetRequiredService<IViewLocator>();
            viewLocator.Open(SidekickViewType.Standard, "/");
        }
        catch (Exception ex)
        {
            logger?.LogCritical(ex, "[App] Error during framework initialization");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task CheckIsAlreadyRunning()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        try
        {
            // Wait a second before starting to listen to interprocess communications.
            await Task.Delay(2000);
            var interprocessService = Program.ServiceProvider.GetRequiredService<IInterprocessService>();
            if (interprocessService.IsAlreadyRunning())
            {
                logger?.LogWarning("[Startup] Another instance of Sidekick is already running. Shutting down.");
                desktop.Shutdown();
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "[App] Error checking if already running");
        }
    }

    private void AttachErrorHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            logger?.LogCritical((Exception)e.ExceptionObject, "Unhandled exception.");
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            logger?.LogCritical(e.Exception, "Unhandled exception.");
        };
    }
}