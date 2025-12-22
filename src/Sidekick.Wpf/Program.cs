using System.IO;
using System.Reflection;
using System.Windows;
using ApexCharts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Sidekick.Apis.Common;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Account;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe2Scout;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Database;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.RegexHotkeys;
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
            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build().Run();

            ServiceProvider = GetServiceProvider();
            ConfigureHardwareAcceleration();

            // We can now launch the WPF application as normal.
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
        catch (TargetInvocationException ex) when (ex.InnerException is WebView2RuntimeNotFoundException)
        {
            MessageBox.Show(@"Microsoft WebView2 Runtime is missing or not installed correctly. Please install the Microsoft WebView2 Runtime, which is required to run this application.

If the issue persists, ensure that Microsoft Edge is fully installed and up-to-date. 

You can download the WebView2 Runtime from the official Microsoft website: https://developer.microsoft.com/en-us/microsoft-edge/webview2/consumer/

If you need more support consider asking on the official Sidekick discord server.");

            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();
            logger.LogCritical(ex, "[Program] WebView2 runtime not found.");
        }
        catch (Exception ex)
        {
            var exceptionMessage = $"Sidekick encountered an unhandled exception: {ex}";
            MessageBox.Show(exceptionMessage.Length > 3000 ? exceptionMessage[..3000] + "..." : exceptionMessage);
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
            .AddSidekickCommonBrowser()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickCommonUi()

            // Apis
            .AddSidekickGitHubApi()
            .AddSidekickCommonApi()
            .AddSidekickPoeApi()
            .AddSidekickPoeAccountApi()
            .AddSidekickPoeTradeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoe2ScoutApi()
            .AddSidekickPoePriceInfoApi()
            .AddSidekickPoeWikiApi()
            .AddSidekickUpdater()

            // Modules
            .AddSidekickChat()
            .AddSidekickDevelopment()
            .AddSidekickRegexHotkeys()
            .AddSidekickGeneral()
            .AddSidekickItems()
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

        services.AddApexCharts();

#pragma warning disable CA1416 // Validate platform compatibility
        services.AddWpfBlazorWebView();
        services.AddBlazorWebViewDeveloperTools();
#pragma warning restore CA1416 // Validate platform compatibility

        return services.BuildServiceProvider();
    }

    private static void ConfigureHardwareAcceleration()
    {
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var useHardwareAcceleration = settingsService.GetBool(SettingKeys.UseHardwareAcceleration).Result;
        if (!useHardwareAcceleration)
        {
            // This changes the variable only for the current running application (does not override outside)
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--disable-gpu --disable-software-rasterizer");
        }
    }
}
