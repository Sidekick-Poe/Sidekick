using ElectronNET.API;
using ElectronNET.API.Entities;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Database;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using Sidekick.Electron;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);

#region Services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder
    .Services

    // Common
    .AddSidekickCommon()
    .AddSidekickCommonBlazor()
    .AddSidekickCommonDatabase()
    .AddSidekickCommonPlatform(
        o =>
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
    .AddSidekickChat()
    .AddSidekickDevelopment()
    .AddSidekickGeneral()
    .AddSidekickMaps()
    .AddSidekickSettings()
    .AddSidekickTrade();
builder.Services.AddSingleton<IApplicationService, ElectronApplicationService>();
builder.Services.AddSingleton<ITrayProvider, ElectronTrayProvider>();
builder.Services.AddSingleton<IViewLocator, ElectronViewLocator>();

#endregion Services

var app = builder.Build();

#region Pipeline

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

var viewLocator = app.Services.GetRequiredService<IViewLocator>();
await viewLocator.Open("/");

// We need to trick Electron into thinking that our app is ready to be opened.
// This makes Electron hide the splashscreen. For us, it means we are ready to initialize and price check :)
var browserWindow = await Electron.WindowManager.CreateWindowAsync(
    new BrowserWindowOptions
    {
        Width = 1,
        Height = 1,
        Frame = false,
        Show = true,
        Transparent = true,
        Fullscreenable = false,
        Minimizable = false,
        Maximizable = false,
        SkipTaskbar = true,
        WebPreferences = new WebPreferences()
        {
            NodeIntegration = false,
        }
    });
browserWindow.WebContents.OnCrashed += (killed) => Electron.App.Exit();
await Task.Delay(50);
browserWindow.Hide();

app.Run();
