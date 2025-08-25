using System.Diagnostics;
using ApexCharts;
using Sidekick.Apis.Common;
using Sidekick.Apis.GitHub;
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
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.RegexHotkeys;
using Sidekick.Modules.Trade;
using Sidekick.Web;
using Sidekick.Web.Services;
using Velopack;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
});

builder.WebHost.UseStaticWebAssets();

#region Services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder.Services

    // Common
    .AddSidekickCommon()
    .AddSidekickCommonBrowser()
    .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
    .AddSidekickCommonUi()

    // Apis
    .AddSidekickGitHubApi()
    .AddSidekickCommonApi()
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
    .AddSidekickMaps()
    .AddSidekickTrade();

builder.Services.AddApexCharts();
builder.Services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
builder.Services.AddSingleton<IViewLocator, WebViewLocator>();
builder.Services.AddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());

#endregion Services

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
VelopackApp.Build().Run(logger);

#region Pipeline

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

// Start the app without blocking.
var runTask = app.RunAsync();

// Open the browser if not debugging.
if (!Debugger.IsAttached)
{
    var applicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    applicationLifetime.ApplicationStarted.Register(() =>
    {
        var browserProvider = app.Services.GetService<IBrowserProvider>();
        if (browserProvider == null) return;

        // Get the first URL the app is listening on
        var url = app.Urls.FirstOrDefault();
        if (!string.IsNullOrEmpty(url))
        {
            browserProvider.OpenUri(new Uri(url));
        }
    });
}

await runTask;
