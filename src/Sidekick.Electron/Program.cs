using ApexCharts;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Sidekick.Apis.Common;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
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
using Sidekick.Electron.Components;
using Sidekick.Electron.Services;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.RegexHotkeys;
using Velopack;

VelopackApp.Build().Run();

var webApplicationOptions = new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    Args = args,
};
var builder = WebApplication.CreateBuilder(webApplicationOptions);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLocalization();

builder.Services.AddElectron();

builder.Services

    // Common
    .AddSidekickCommon()
    .AddSidekickCommonBrowser()
    .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
    .AddSidekickCommonUi()

    // Apis
    .AddSidekickGitHubApi()
    .AddSidekickCommonApi()
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

    // Platform needs to be at the end
    .AddSidekickCommonPlatform(o =>
    {
        o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
        o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
    });

builder.Services.AddApexCharts();
builder.Services.AddSidekickInitializableService<IApplicationService, ElectronApplicationService>();
builder.Services.AddSingleton<IViewLocator, ElectronViewLocator>();
builder.Services.AddSingleton(sp => (ElectronViewLocator)sp.GetRequiredService<IViewLocator>());

builder.UseElectron(args, async () =>
{
    var options = new BrowserWindowOptions
    {
        Show = false,
        IsRunningBlazor = true,
        Frame = false,
        HasShadow = false,
        Icon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico"),
    };

    if (OperatingSystem.IsWindows())
    {
        options.ThickFrame = false;
        options.RoundedCorners = false;
    }

    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);

    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

var app = builder.Build();

app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<ElectronHostPage>().AddInteractiveServerRenderMode();

app.Run();