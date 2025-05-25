using System.IO;
using System.Reflection;
using System.Windows;
using ApexCharts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Sidekick.Apis.Common;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe.Account;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe2Scout;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Database;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Windows.Interprocess;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Modules.Chat;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.RegexHotkeys;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;
using Sidekick.Wpf.Services;
using Velopack;

namespace Sidekick.Wpf;

public class Program
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    // Since WPF has an "automatic" Program.Main, we need to create our own.
    // In order for this to work, you must also add the following to your .csproj:
    // <StartupObject>Sidekick.Wpf.App</StartupObject>
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            ServiceProvider = GetServiceProvider();
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build().Run(logger);

            // We can now launch the WPF application as normal.
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
        catch (TargetInvocationException ex) when (ex.InnerException is WebView2RuntimeNotFoundException)
        {
            MessageBox.Show("Microsoft WebView2 Runtime is missing or not installed correctly. Please install the Microsoft WebView2 Runtime, which is required to run this application. \n\nIf the issue persists, ensure that Microsoft Edge is fully installed and up-to-date. \n\nYou can download the WebView2 Runtime from the official Microsoft website: https://developer.microsoft.com/en-us/microsoft-edge/webview2/consumer/\n\nIf you need more support consider asking on the official Sidekick discord server.\n");
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();
            logger.LogCritical(ex, "[Program] WebView2 runtime not found.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unhandled exception: " + ex);
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();
            logger.LogCritical(ex, "[Program] Unhandled exception.");
        }
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
            .AddSidekickCommonUi()

            // Apis
            .AddSidekickGitHubApi()
            .AddSidekickCommonApi()
            .AddSidekickPoeAccountApi()
            .AddSidekickPoeTradeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoe2ScoutApi()
            .AddSidekickPoePriceInfoApi()
            .AddSidekickPoeWikiApi()
            .AddSidekickUpdater()

            // Modules
            .AddSidekickChat()
            .AddSidekickRegexHotkeys()
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
        services.AddSingleton<WpfBrowserWindowProvider>();
        services.AddSingleton<WpfCloudflareHandler>();

        services.AddApexCharts();

#pragma warning disable CA1416 // Validate platform compatibility
        services.AddWpfBlazorWebView();
        services.AddBlazorWebViewDeveloperTools();
#pragma warning restore CA1416 // Validate platform compatibility

        return services.BuildServiceProvider();
    }
}
