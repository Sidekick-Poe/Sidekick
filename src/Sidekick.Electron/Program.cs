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
using Sidekick.Electron.Services;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.RegexHotkeys;
using Velopack;

VelopackApp.Build().Run();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
});

#region Services

builder.Services
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddElectron();
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
    .AddSidekickItems();

builder.Services.AddApexCharts();
builder.Services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
builder.Services.AddSingleton<IViewLocator, WebViewLocator>();
builder.Services.AddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());

builder.UseElectron(args, async () =>
{
    var options = new BrowserWindowOptions
    {
        Show = false,
        IsRunningBlazor = true,
    };

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
    {
        options.AutoHideMenuBar = true;
    }

    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);
    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

#endregion Services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Main>()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();