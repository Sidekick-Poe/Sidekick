using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia;

public partial class App : Application
{
    private Process? webProcess;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // new Uri($"localhost:{ServerAppHost.Port}"),
            var url = "http://localhost:5000";
            var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sidekick.Web.exe");
            if (File.Exists(exePath))
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"--urls {url}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

#if DEBUG
                startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";
#endif

                webProcess = Process.Start(startInfo);
            }

            desktop.MainWindow = new MainWindow(SidekickViewType.Standard, url);

            desktop.ShutdownRequested += (_, _) =>
            {
                if (webProcess is { HasExited: false })
                {
                    webProcess.Kill();
                }
            };
        }

        if (webProcess == null)
        {
        // logger?.LogCritical("[App] Unsupported application type.");
        // throw new Exception("Unsupported application type.");
        }

        try
        {
             // logger = host.Application.Services.GetService<ILogger<App>>();
             AttachErrorHandlers();
             // _ = CheckIsAlreadyRunning();

             // Initialize host-level services (tray icon, language change listener)
             // var appService = host.Application.Services.GetRequiredService<IApplicationService>();
             // _ = appService.Initialize();

             // var viewLocator = host.Application.Services.GetRequiredService<IViewLocator>();
             // viewLocator.Open(SidekickViewType.Standard, "/");
        }
        catch (Exception ex)
        {
            // logger?.LogCritical(ex, "[App] Error during framework initialization");
        }

        base.OnFrameworkInitializationCompleted();
    }

    // private async Task CheckIsAlreadyRunning()
    // {
    //     if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

    //     try
    //     {
    //         // Wait a second before starting to listen to interprocess communications.
    //         await Task.Delay(2000);
    //         var interprocessService = Program.ServiceProvider.GetRequiredService<IInterprocessService>();
    //         if (interprocessService.IsAlreadyRunning())
    //         {
    //             logger?.LogWarning("[Startup] Another instance of Sidekick is already running. Shutting down.");
    //             desktop.Shutdown();
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         logger?.LogError(ex, "[App] Error checking if already running");
    //     }
    // }

    private void AttachErrorHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            // logger?.LogCritical((Exception)e.ExceptionObject, "Unhandled exception.");
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            // logger?.LogCritical(e.Exception, "Unhandled exception.");
        };
    }
}
