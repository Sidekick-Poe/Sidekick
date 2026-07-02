using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Sidekick.Avalonia.Components;
using Sidekick.Avalonia.Services;
using Sidekick.Common;
using Sidekick.Common.Blazor.Home;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using Sidekick.Web;

namespace Sidekick.Avalonia;

public partial class App : Application
{
    private static ServerAppHost? _serverAppHost;
    public static ServerAppHost ServerAppHost
    {
        get => _serverAppHost ?? throw new Exception("ServerAppHost not initialized.");
        private set => _serverAppHost = value;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServerAppHost = new ServerAppHost(SidekickApplicationType.Avalonia);

            var tcs = new TaskCompletionSource();
            _ = Task.Run(async () =>
            {
                ServerAppHost.Start(services =>
                {
                    services.TryAddSingleton<IViewLocator, AvaloniaViewLocator>();
                    services.TryAddSingleton<IApplicationService, AvaloniaApplicationService>();
                    services.TryAddSingleton<AvaloniaCultureHandler>();
                    services.TryAddSingleton<AvaloniaDialogsHandler>();
                });
                tcs.TrySetResult();
                await ServerAppHost.RunTask;
            });

            tcs.Task.GetAwaiter().GetResult();

            // Triggers the constructor of specific handler
            _ = ServerAppHost.Application.Services.GetRequiredService<AvaloniaCultureHandler>();
            _ = ServerAppHost.Application.Services.GetRequiredService<AvaloniaDialogsHandler>();

            var viewLocator = ServerAppHost.Application.Services.GetRequiredService<IViewLocator>();
            viewLocator.Open(SidekickViewType.Splash, "/");

            desktop.ShutdownRequested += (_, _) =>
            {
                ServerAppHost.Dispose();
            };

            InitializeTray();
        }

        if (_serverAppHost == null)
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

    private void InitializeTray()
    {
        var resources = ServerAppHost.Application.Services.GetRequiredService<IStringLocalizer<HomeResources>>();
        var tray = new TrayIcons
        {
            new TrayIcon
            {
                Icon = new WindowIcon("Assets/favicon.ico"),
                Command = new SimpleCommand(() =>
                {
                    var viewLocator = ServerAppHost.Application.Services.GetRequiredService<IViewLocator>();
                    viewLocator.Open(SidekickViewType.Standard, "/home");
                }),
                Menu = new NativeMenu
                {
                    new NativeMenuItem("Sidekick - " + ServerAppHost.Application.Services.GetRequiredService<IApplicationService>().GetVersion())
                    {
                        IsEnabled = false,
                    },
                    new NativeMenuItem(resources["Home"])
                    {
                        Command = new SimpleCommand(() =>
                        {
                            var viewLocator = ServerAppHost.Application.Services.GetRequiredService<IViewLocator>();
                            viewLocator.Open(SidekickViewType.Standard, "/home");
                        }),
                    },
                    new NativeMenuItem(resources["Open_Website"])
                    {
                        Command = new SimpleCommand(() =>
                        {
                            var browserProvider = ServerAppHost.Application.Services.GetRequiredService<IBrowserProvider>();
                            browserProvider.OpenUri(browserProvider.SidekickWebsite);
                        }),
                    },
                    new NativeMenuItem(resources["Exit"])
                    {
                        Command = new SimpleCommand(() =>
                        {
                            var applicationService = ServerAppHost.Application.Services.GetRequiredService<IApplicationService>();
                            applicationService.Shutdown();
                        }),
                    },
                }
            }
        };

        TrayIcon.SetIcons(Current!, tray);
    }
}
