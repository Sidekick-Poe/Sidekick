using ApexCharts;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common.Updater;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Database;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Linux.Platform;
using Sidekick.Mock;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;
using Sidekick.Linux;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Velopack;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
});

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
    .AddSidekickCommonUi()
    .AddSidekickCommonPlatform(o =>
    {
        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
        o.WindowsIconPath = iconPath;
        o.OsxIconPath = iconPath;
    })
    .AddSingleton<IInterprocessService, InterprocessService>()

    // Apis
    .AddSidekickGitHubApi()
    .AddSidekickPoeApi()
    .AddSidekickPoeNinjaApi()
    .AddSidekickPoePriceInfoApi()
    .AddSidekickPoeWikiApi()
    .AddSidekickUpdater()

    // Modules
    .AddSidekickChat()
    .AddSidekickDevelopment()
    .AddSidekickGeneral()
    .AddSidekickMaps()
    .AddSidekickTrade()
    .AddSidekickWealth();

builder.Services.AddApexCharts();

builder.Services.AddSingleton<IApplicationService, MockApplicationService>();
builder.Services.AddSingleton<ITrayProvider, MockTrayProvider>();
builder.Services.AddSingleton<IViewLocator, X11ViewLocator>();
builder.Services.AddSidekickInitializableService<IProcessProvider, X11ProcessProvider>();

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

VelopackApp.Build().Run();

app.Run();
