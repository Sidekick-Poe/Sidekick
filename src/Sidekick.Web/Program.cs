using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Mock;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

try
{
    builder.Configuration.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);
}
catch (Exception) { }

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
    .AddSidekickCommon(builder.Configuration)
    .AddSidekickCommonBlazor()
    .AddSingleton<IInterprocessService, InterprocessService>()
    // .AddSidekickCommonPlatform(o =>
    // {
    //     o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
    //     o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
    // })

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
    .AddSidekickMaps()
    .AddSidekickSettings()
    .AddSidekickTrade()
    .AddSidekickWealth()

    // Mocks
    .AddSidekickMocks();

builder.Services.AddSingleton<IApplicationService, MockApplicationService>();
builder.Services.AddSingleton<ITrayProvider, MockTrayProvider>();
builder.Services.AddSingleton<IViewLocator, MockViewLocator>();

#endregion Services

var app = builder.Build();

#region Pipeline

app.Services.GetRequiredService<IInterprocessService>().StartReceiving();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

app.Run();
