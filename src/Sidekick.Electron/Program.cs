using ElectronNET.API;
using ElectronNET.API.Entities;
using Sidekick.Electron;

// VelopackApp.Build().Run();

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddElectron();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();

/*

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
*/

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

app.UseRouting();

app.MapRazorPages();

app.MapRazorComponents<ElectronTest>()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();