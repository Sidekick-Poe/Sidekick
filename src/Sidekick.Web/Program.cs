using Sidekick;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Mock;
using Sidekick.Modules.Settings;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

builder.Configuration.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);

#endregion Configuration

#region Services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();

builder.Services.AddSidekick(builder.Configuration);
builder.Services.AddSingleton<IApplicationService, MockApplicationService>();
builder.Services.AddSingleton<ITrayProvider, MockTrayProvider>();
builder.Services.AddSingleton<IViewLocator, MockViewLocator>();

#endregion Services

var app = builder.Build();

#region Pipeline

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#endregion Pipeline

app.Run();
