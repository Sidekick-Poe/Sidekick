using System.Diagnostics;
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
using Sidekick.Common.Dialogs;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using Sidekick.Web;

namespace Sidekick.Avalonia;

public partial class App : Application
{
    public static ServerAppHost? ServerAppHost { get; private set; }
    public static ServerAppHost RequiredServerAppHost => ServerAppHost ?? throw new Exception("ServerAppHost not initialized.");

    private int isHandlingFatalException;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            {
                throw new Exception("Unsupported application type.");
            }

            AppDomain.CurrentDomain.UnhandledException += (_, e)
                => HandleException(e.ExceptionObject as Exception ?? new Exception("Unknown exception"));
            TaskScheduler.UnobservedTaskException += (_, e)
                => HandleException(e.Exception);

            ServerAppHost = new ServerAppHost(SidekickApplicationType.Avalonia);
            var tcs = new TaskCompletionSource();
            _ = Task.Run(async () =>
            {
                try
                {
                    ServerAppHost.Start(services =>
                    {
                        services.TryAddSingleton<IViewLocator, AvaloniaViewLocator>();
                        services.TryAddSingleton<IApplicationService, AvaloniaApplicationService>();
                        services.TryAddSingleton<AvaloniaCultureHandler>();
                        services.TryAddSingleton<AvaloniaDialogsHandler>();
                        services.TryAddSingleton<AvaloniaBrowserDialogHandler>();
                        services.TryAddSingleton<AvaloniaTransparentDialogProvider>();
                    });
                    tcs.TrySetResult();
                    await ServerAppHost.RunTask;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            });

            tcs.Task.GetAwaiter().GetResult();

            _ = CheckIsAlreadyRunning();

            // Triggers the constructor of specific handler
            _ = RequiredServerAppHost.Application.Services.GetRequiredService<AvaloniaCultureHandler>();
            _ = RequiredServerAppHost.Application.Services.GetRequiredService<AvaloniaDialogsHandler>();
            _ = RequiredServerAppHost.Application.Services.GetRequiredService<AvaloniaBrowserDialogHandler>();
            _ = RequiredServerAppHost.Application.Services.GetRequiredService<AvaloniaTransparentDialogProvider>();

            var viewLocator = RequiredServerAppHost.Application.Services.GetRequiredService<IViewLocator>();
            viewLocator.Open(SidekickViewType.Splash, "/");

            desktop.ShutdownRequested += (_, _) =>
            {
                RequiredServerAppHost.Dispose();
            };

            InitializeTray();

            if (ServerAppHost == null)
            {
                throw new Exception("Could not initialize server app host.");
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private async Task CheckIsAlreadyRunning()
    {
        try
        {
            // Wait a second before starting to listen to interprocess communications.
            await Task.Delay(2000);
            var interprocessService = ServerAppHost?.Application.Services.GetService<IInterprocessService>();
            if (interprocessService?.IsAlreadyRunning() == true)
            {
                var window = new DialogWindow(DialogProvider.Type.Ok,
                                              "Another instance of Sidekick is already running. Make sure to close all instances of Sidekick inside the Task Manager.");
                window.Show();
                await window.Task;
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void InitializeTray()
    {
        var resources = RequiredServerAppHost.Application.Services.GetRequiredService<IStringLocalizer<HomeResources>>();
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "favicon.ico");

        var trayIcon = new TrayIcon{
                Command = new SimpleCommand(() =>
                {
                    var viewLocator = RequiredServerAppHost.Application.Services.GetRequiredService<IViewLocator>();
                    viewLocator.Open(SidekickViewType.Standard, "/home");
                }),
                Menu = new NativeMenu
                {
                    new NativeMenuItem("Sidekick - " + RequiredServerAppHost.Application.Services.GetRequiredService<IApplicationService>().GetVersion())
                    {
                        IsEnabled = false,
                    },
                    new NativeMenuItem(resources["Home"])
                    {
                        Command = new SimpleCommand(() =>
                        {
                            var viewLocator = RequiredServerAppHost.Application.Services.GetRequiredService<IViewLocator>();
                            viewLocator.Open(SidekickViewType.Standard, "/home");
                        }),
                    },
                    new NativeMenuItem(resources["Open_Website"])
                    {
                        Command = new SimpleCommand(() =>
                        {
                            var browserProvider = RequiredServerAppHost.Application.Services.GetRequiredService<IBrowserProvider>();
                            browserProvider.OpenUri(browserProvider.SidekickWebsite);
                        }),
                    },
                    new NativeMenuItem(resources["Exit"])
                    {
                        Command = new SimpleCommand(() =>
                        {
                            var applicationService = RequiredServerAppHost.Application.Services.GetRequiredService<IApplicationService>();
                            applicationService.Shutdown();
                        }),
                    },
                }
        };

        if (File.Exists(iconPath))
        {
            trayIcon.Icon = new WindowIcon(iconPath);
        }

        var tray = new TrayIcons
        {
            trayIcon,
        };

        TrayIcon.SetIcons(Current!, tray);
    }

    private void HandleException(Exception ex)
    {
        if (Interlocked.Exchange(ref isHandlingFatalException, 1) == 1)
        {
            return;
        }

        _ = Dispatcher.InvokeAsync(async () =>
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            var logger = ServerAppHost?.Application.Services.GetService<ILogger<App>>();
            logger?.LogCritical(ex, "[App] Application critical error");

            var keyboardProvider = ServerAppHost?.Application.Services.GetService<IInputProvider>();
            keyboardProvider?.UnregisterHooks();

            var viewLocator = ServerAppHost?.Application.Services.GetService<IViewLocator>();
            viewLocator?.Close(SidekickViewType.Overlay);
            viewLocator?.Close(SidekickViewType.Splash);
            viewLocator?.Close(SidekickViewType.Standard);

            var window = new DialogWindow(DialogProvider.Type.Ok, $"Unexpected error: {ex.Message}");
            window.Show();

            await window.Task;

            Environment.Exit(0);
        });
    }
}
