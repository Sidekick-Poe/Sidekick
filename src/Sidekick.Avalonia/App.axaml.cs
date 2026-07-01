using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using Sidekick.Web;
using Sidekick.Web.Services;

namespace Sidekick.Avalonia;

public partial class App : Application
{
    private ServerAppHost? serverAppHost;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            serverAppHost = new ServerAppHost(SidekickApplicationType.Avalonia);
            var tcs = new TaskCompletionSource<string?>();

            _ = Task.Run(async () =>
            {
                serverAppHost.Start(services =>
                {
                    services.TryAddSingleton<IViewLocator, WebViewLocator>();
                    services.TryAddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());
                    services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
                });
                tcs.TrySetResult(serverAppHost.Application.Urls.FirstOrDefault());
                await serverAppHost.RunTask;
            });

            var url = tcs.Task.GetAwaiter().GetResult();
            if (url != null)
            {
                var window = new MainWindow(serverAppHost.Application.Services);
                _ = window.OpenView(url);
                desktop.MainWindow = window;
            }

            desktop.ShutdownRequested += (_, _) =>
            {
                serverAppHost.Dispose();
            };
        }

        if (serverAppHost == null)
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
