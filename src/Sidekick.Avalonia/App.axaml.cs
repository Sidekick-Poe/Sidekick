using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Sidekick.Avalonia;

public partial class App : Application
{
    private readonly CancellationTokenSource serverTokenSource = new();
    // private ServerAppHost? host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
//       host = new ServerAppHost(SidekickApplicationType.Avalonia);
//       host.Start(configureServices: services => {
//                      services.AddSingleton<IApplicationService, AvaloniaApplicationService>();
//                      services.AddSingleton<IViewLocator, AvaloniaViewLocator>();
//                  },
//                  cancellationToken: serverTokenSource.Token);

            // desktop.MainWindow = new MainWindow(SidekickViewType.Standard);

            desktop.MainWindow = new Window()
            {
                Content = new NativeWebView()
                {
                    Source = new Uri($"localhost:5000"), // new Uri($"localhost:{ServerAppHost.Port}"),
                }
            };

            // desktop.ShutdownRequested += (_, _) =>
            // {
            //     serverTokenSource.CancelAsync().ContinueWith(_ =>
            //     {
            //         host.Dispose();
            //         host = null;
            //     });
            // };
        }

        // if (host == null)
        // {
        // logger?.LogCritical("[App] Unsupported application type.");
        // throw new Exception("Unsupported application type.");
        // }

        try
        {
            // logger = host.Application.Services.GetService<ILogger<App>>();
            // AttachErrorHandlers();
            // _ = CheckIsAlreadyRunning();

            // Initialize host-level services (tray icon, language change listener)
            // var appService = host.Application.Services.GetRequiredService<IApplicationService>();
            // _ = appService.Initialize();
//
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
