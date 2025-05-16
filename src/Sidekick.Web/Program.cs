using System.Diagnostics;
using ApexCharts;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe2Scout;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Browser;
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
using Velopack;

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
    .AddSidekickWealth();

builder.Services.AddApexCharts();
builder.Services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
builder.Services.AddSingleton<IViewLocator, WebViewLocator>();
builder.Services.AddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());

#endregion Services

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
VelopackApp.Build().Run(logger);

#region Pipeline

app.Services.GetRequiredService<IInterprocessService>().StartReceiving();

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
        if (browserProvider != null)
        {
            // Get the first URL the app is listening on
            var url = app.Urls.FirstOrDefault();
            if (!string.IsNullOrEmpty(url))
            {
                browserProvider.OpenUri(new Uri(url));
            }
        }
    });
}

await runTask;
