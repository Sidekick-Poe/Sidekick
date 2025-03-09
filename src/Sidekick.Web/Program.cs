using ApexCharts;
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
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Modules.Chat;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;
using Sidekick.Web;
using Sidekick.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

#region Services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder.Services

    // Common
    .AddSidekickCommon()
    .AddSidekickCommonBlazor()
    .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
    .AddSidekickCommonInterprocess()
    .AddSidekickCommonUi()

    // .AddSidekickCommonPlatform(o =>
    // {
    //     o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
    //     o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
    // })

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
    .AddSidekickWealth();

builder.Services.AddApexCharts();
builder.Services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
builder.Services.AddSingleton<IViewLocator, WebViewLocator>();

#endregion Services

var app = builder.Build();

#region Pipeline

app.Services.GetRequiredService<IInterprocessService>().StartReceiving();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

app.Run();
