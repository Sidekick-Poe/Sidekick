using System.Diagnostics;
using ApexCharts;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
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
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Data;
using Sidekick.Data.Builder;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.RegexHotkeys;
using Sidekick.Modules.Wealth;
using Sidekick.PhotinoBlazor.Services;
using Velopack;

namespace Sidekick.PhotinoBlazor;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        VelopackApp.Build().Run();

        var photinoBlazorAppBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        AddServices(photinoBlazorAppBuilder.Services);
        photinoBlazorAppBuilder.Services.BuildServiceProvider();
        photinoBlazorAppBuilder.RootComponents.Add<SidekickPhotinoBlazorWrapper>("#app");

        var app = photinoBlazorAppBuilder.Build();

        app.MainWindow
            .SetIconFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico"))
            .SetTitle("Sidekick")
            .SetUserAgent("Sidekick")
            .Center()
            .SetNotificationsEnabled(false); // https://github.com/tryphotino/photino.NET/issues/85

        if (!Debugger.IsAttached)
        {
            app.MainWindow.ContextMenuEnabled = false;
            app.MainWindow.DevToolsEnabled = false;
        }

        app.Run();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddLocalization();

        services

            // Common
            .AddSidekickCommon(SidekickApplicationType.Photino)
            .AddSidekickCommonBrowser()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickCommonUi()

            // Data
            .AddSidekickData()
            .AddSidekickDataBuilder()

            // Apis
            .AddSidekickGitHubApi()
            .AddSidekickCommonApi()
            .AddSidekickPoeAccountApi()
            .AddSidekickPoeApi()
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
            })

            // Photino.Blazor
            .AddScoped(sp => new HttpClient(new PhotinoHttpHandler(sp.GetService<PhotinoBlazorApp>())) { BaseAddress = new Uri(PhotinoWebViewManager.AppBaseUri) })
            .AddSingleton<PhotinoBlazorApp>()
            .AddBlazorWebView();

        services.AddSidekickInitializableService<IApplicationService, PhotinoBlazorApplicationService>();
        services.AddSingleton<IViewLocator, PhotinoBlazorViewLocator>();
        services.AddSingleton(sp => (PhotinoBlazorViewLocator)sp.GetRequiredService<IViewLocator>());
        //services.AddSingleton<WpfCloudflareHandler>();

        services.AddApexCharts();
    }
}
