using System;
using System.IO;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using Sidekick.Electron.Services;
using Sidekick.Electron.Views;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Cheatsheets;
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
    .AddSidekickGeneral()
    .AddSidekickInitialization()
    .AddSidekickMaps()
    .AddSidekickSettings(builder.Configuration)
    .AddSidekickTrade()
    .AddSidekickUpdate();

builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder.Services.AddSingleton<IApplicationService, ApplicationService>();
builder.Services.AddSingleton<IViewLocator, ViewLocator>();
builder.Services.AddSingleton((sp) => (ViewLocator)sp.GetService<IViewLocator>());
builder.Services.AddSingleton((sp) => (ViewInstance)sp.GetService<IViewInstance>());
builder.Services.AddScoped<IViewInstance, ViewInstance>();

#endregion Services

var app = builder.Build();

#region Pipeline

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

#endregion Pipeline

await app.StartAsync();

#region Electron

// Electron stuff
ElectronNET.API.Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
ElectronNET.API.Electron.WindowManager.IsQuitOnWindowAllClosed = false;

// We need to trick Electron into thinking that our app is ready to be opened.
// This makes Electron hide the splashscreen. For us, it means we are ready to initialize and price check :)
var browserWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
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
browserWindow.WebContents.OnCrashed += (killed) => ElectronNET.API.Electron.App.Exit();
await Task.Delay(50);
browserWindow.Close();

// Initialize Sidekick
var viewLocator = app.Services.GetService<IViewLocator>();
await viewLocator.Open("/update");

#endregion Electron

app.WaitForShutdown();
