using ApexCharts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe2Scout;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Database;
using Sidekick.Common.Interprocess;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Modules.Chat;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;
using Sidekick.PhotinoBlazor.Services;
using Velopack;

namespace Sidekick.PhotinoBlazor;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var photinoBlazorAppBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        AddServices(photinoBlazorAppBuilder.Services);
        var serviceProvider = photinoBlazorAppBuilder.Services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        VelopackApp.Build().Run(logger);

        photinoBlazorAppBuilder.RootComponents.Add<SidekickPhotinoBlazorWrapper>("#app");

        var photinoBlazorApp = photinoBlazorAppBuilder.Build();

        photinoBlazorApp.MainWindow
            .SetIconFile("wwwroot/favicon.ico")
            .SetTitle("Sidekick");

        photinoBlazorApp.Run();
    }

    private static void AddServices(IServiceCollection services)
    {
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
            .AddSidekickPoe2ScoutApi()
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
