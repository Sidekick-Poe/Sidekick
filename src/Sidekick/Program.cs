using ElectronNET.API;
using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Game;
using Sidekick.Common.Platform;
using Sidekick.Electron;
using Sidekick.Mock;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Initialization;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Update;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);

#region Configuration

builder.Configuration.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);

#endregion Configuration

#region Services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder.Services
    // MudBlazor
    .AddMudServices()
    .AddMudBlazorDialog()
    .AddMudBlazorSnackbar()
    .AddMudBlazorResizeListener()
    .AddMudBlazorScrollListener()
    .AddMudBlazorScrollManager()
    .AddMudBlazorJsApi()

    // Common
    .AddSidekickCommon()
    .AddSidekickCommonGame()
    .AddSidekickCommonPlatform(o =>
    {
        o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
        o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
    })

    // Apis
    .AddSidekickGitHubApi()
    .AddSidekickPoeApi()
    .AddSidekickPoeNinjaApi()
    .AddSidekickPoePriceInfoApi()
    .AddSidekickPoeWikiApi()

    // Modules
    .AddSidekickAbout()
    .AddSidekickChat()
    .AddSidekickCheatsheets()
    .AddSidekickDevelopment()
    .AddSidekickGeneral()
    .AddSidekickInitialization()
    .AddSidekickMaps()
    .AddSidekickSettings(builder.Configuration)
    .AddSidekickTrade()
    .AddSidekickUpdate()

    // Mock
    .AddSingleton<IApplicationService, MockApplicationService>()
    .AddSingleton<IViewLocator, MockViewLocator>();

if (HybridSupport.IsElectronActive)
{
    builder.Services.AddSingleton<IViewLocator, ElectronViewLocator>();
}
else
{
    builder.Services.AddSingleton<IViewLocator, MockViewLocator>();
}

builder.Services.AddHttpClient();
builder.Services.AddLocalization();

#endregion Services

var app = builder.Build();

#region Pipeline

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

if (HybridSupport.IsElectronActive)
{
    var viewLocator = app.Services.GetRequiredService<IViewLocator>();
    await viewLocator.Open("/update");
}

app.Run();
