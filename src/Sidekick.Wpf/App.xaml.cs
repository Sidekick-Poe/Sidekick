using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ApexCharts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Database;
using Sidekick.Common.Interprocess;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Modules.Chat;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;
using Sidekick.Wpf.Services;
using Velopack;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    private readonly ILogger<App> logger;
    private readonly ISettingsService settingsService;
    private readonly IInterprocessService interprocessService;

    public App()
    {
        VelopackApp.Build().Run();

        DisableWindowsTheme();
        ServiceProvider = GetServiceProvider();
        logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        interprocessService = ServiceProvider.GetRequiredService<IInterprocessService>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var currentDirectory = Directory.GetCurrentDirectory();
        var settingDirectory = settingsService.GetString(SettingKeys.CurrentDirectory).Result;
        if (string.IsNullOrEmpty(settingDirectory) || settingDirectory != currentDirectory)
        {
            logger.LogDebug("[Startup] Current Directory set to: {0}", currentDirectory);
            settingsService.Set(SettingKeys.CurrentDirectory, currentDirectory).Wait();
            settingsService.Set(SettingKeys.WealthEnabled, false).Wait();
        }

        _ = HandleInterprocessCommunications(e);

        AttachErrorHandlers();
        interprocessService.StartReceiving();
        var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
        _ = viewLocator.Open("/");
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
            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
            await viewLocator.CloseAll();
            var sidekickDialogs = ServiceProvider.GetRequiredService<ISidekickDialogs>();
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

    private static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLocalization();

        services

            // Common
            .AddSidekickCommon()
            .AddSidekickCommonBlazor()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickCommonInterprocess()
            .AddSidekickCommonUi()

            // Apis
            .AddSidekickGitHubApi()
            .AddSidekickPoeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoePriceInfoApi()
            .AddSidekickPoeWikiApi()
            .AddSidekickUpdater()

            // Modules
            .AddSidekickChat()
            .AddSidekickGeneral()
            .AddSidekickMaps()
            .AddSidekickTrade()
            .AddSidekickWealth()

            // Platform needs to be at the end
            .AddSidekickCommonPlatform(o =>
            {
                o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
                o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
            });

        services.AddSidekickInitializableService<IApplicationService, WpfApplicationService>();
        services.AddSingleton<IViewLocator, WpfViewLocator>();
        services.AddSingleton(sp => (WpfViewLocator)sp.GetRequiredService<IViewLocator>());

        services.AddApexCharts();

#pragma warning disable CA1416 // Validate platform compatibility
        services.AddWpfBlazorWebView();
        services.AddBlazorWebViewDeveloperTools();
#pragma warning restore CA1416 // Validate platform compatibility

        return services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider != null!)
        {
            ServiceProvider.Dispose();
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
