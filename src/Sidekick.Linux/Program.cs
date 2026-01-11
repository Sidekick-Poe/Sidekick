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
using Sidekick.Linux.Diagnostics;
using Sidekick.Linux.Platform;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.RegexHotkeys;
using Sidekick.Modules.Wealth;
using Velopack;

// Ubuntu 24.04 blocks unprivileged user namespaces, so default to disabling WebKit sandbox
// and allow explicit opt-in via SIDEKICK_WEBKIT_SANDBOX=1.
ConfigureWebKitSandbox();

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
builder.Services.AddSidekickInitializableService<LinuxDiagnosticsService, LinuxDiagnosticsService>();
builder.Services.AddSingleton<IViewLocator, X11ViewLocator>();
builder.Services.AddSingleton(sp => (X11ViewLocator)sp.GetRequiredService<IViewLocator>());
builder.Services.AddSingleton<IOverlayInputRegionService>(sp => sp.GetRequiredService<X11ViewLocator>());
builder.Services.AddSingleton<IOverlayVisibilityService>(sp => sp.GetRequiredService<X11ViewLocator>());
builder.Services.AddSidekickInitializableService<OverlayWidgetService, OverlayWidgetService>();
builder.Services.AddSingleton<IOverlayStateProvider, OverlayStateProvider>();
if (string.IsNullOrEmpty(startupUrl))
{
    builder.Services.AddSidekickInitializableService<LinuxOverlayFocusWatcher, LinuxOverlayFocusWatcher>();
}
builder.Services.AddSidekickInitializableService<LinuxPoeWindowWatcher, LinuxPoeWindowWatcher>();
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

static void ConfigureWebKitSandbox()
{
    var sandboxEnabled = ReadBoolEnv("SIDEKICK_WEBKIT_SANDBOX");
    if (sandboxEnabled == true)
    {
        SetEnvIfMissing("WEBKIT_DISABLE_SANDBOX", "0");
        SetEnvIfMissing("WEBKIT_FORCE_SANDBOX", "1");
    }
    else
    {
        SetEnvIfMissing("WEBKIT_DISABLE_SANDBOX", "1");
        SetEnvIfMissing("WEBKIT_FORCE_SANDBOX", "0");
    }

    SetEnvIfMissing("GTK_USE_PORTAL", "0");
    SetEnvIfMissing("GIO_USE_VFS", "local");
}

static bool? ReadBoolEnv(string key)
{
    var value = Environment.GetEnvironmentVariable(key);
    if (string.IsNullOrWhiteSpace(value))
    {
        return null;
    }

    return value.Trim().ToLowerInvariant() switch
    {
        "1" => true,
        "true" => true,
        "yes" => true,
        "on" => true,
        "0" => false,
        "false" => false,
        "no" => false,
        "off" => false,
        _ => null,
    };
}

static void SetEnvIfMissing(string key, string value)
{
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
    {
        Environment.SetEnvironmentVariable(key, value);
    }
}
