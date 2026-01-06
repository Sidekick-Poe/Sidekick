using ApexCharts;
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
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Overlay;
using Sidekick.Common.Ui.Views;
using Sidekick.Common.Updater;
using Sidekick.Linux;
using Sidekick.Linux.Platform;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.RegexHotkeys;
using Sidekick.Modules.Wealth;
using Velopack;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
});

var startupUrl = ResolveStartupUrl(args);

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
    .AddSingleton<IInterprocessService, InterprocessService>()

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

    // Platform (after modules so input handlers are registered)
    .AddSidekickCommonPlatform(o =>
    {
        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
        o.WindowsIconPath = iconPath;
        o.OsxIconPath = iconPath;
    });

builder.Services.AddApexCharts();

builder.Services.AddSidekickInitializableService<IApplicationService, LinuxApplicationService>();
builder.Services.AddSidekickInitializableService<LinuxOverlayDefaultsInitializer, LinuxOverlayDefaultsInitializer>();
builder.Services.AddSingleton<IViewLocator, X11ViewLocator>();
builder.Services.AddSingleton(sp => (X11ViewLocator)sp.GetRequiredService<IViewLocator>());
builder.Services.AddSingleton<IOverlayInputRegionService>(sp => sp.GetRequiredService<X11ViewLocator>());
builder.Services.AddSingleton<IOverlayVisibilityService>(sp => sp.GetRequiredService<X11ViewLocator>());
builder.Services.AddSidekickInitializableService<OverlayWidgetService, OverlayWidgetService>();
if (string.IsNullOrEmpty(startupUrl))
{
    builder.Services.AddSidekickInitializableService<LinuxOverlayFocusWatcher, LinuxOverlayFocusWatcher>();
}
if (!string.IsNullOrEmpty(startupUrl))
{
    builder.Services.AddSingleton(new LinuxStartupWindowOptions(startupUrl));
    builder.Services.AddHostedService<LinuxStartupWindowService>();
}
builder.Services.AddSidekickInitializableService<IProcessProvider, X11ProcessProvider>();

#endregion Services

var app = builder.Build();

#region Pipeline

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

VelopackApp.Build().Run();

app.Run();

static string? ResolveStartupUrl(string[] args)
{
    foreach (var arg in args)
    {
        if (arg.Equals("--initialize", StringComparison.OrdinalIgnoreCase))
        {
            return "/initialize";
        }

        const string prefix = "--startup-url=";
        if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return arg[prefix.Length..];
        }
    }

    var envUrl = Environment.GetEnvironmentVariable("SIDEKICK_STARTUP_URL");
    return string.IsNullOrWhiteSpace(envUrl) ? null : envUrl;
}
