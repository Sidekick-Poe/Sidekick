using ApexCharts;
using Sidekick.Apis.Common;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe2Scout;
using Sidekick.Apis.PoeDb;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.AvaloniaServer.Services;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Database;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Data;
using Sidekick.Data.Builder;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Data;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Items;
using Sidekick.Modules.Logs;
using Sidekick.Modules.RegexHotkeys;
using Sidekick.Modules.Updater;
using Sidekick.Modules.Wealth;

namespace Sidekick.AvaloniaServer;

/// <summary>
/// Manages the embedded Blazor Server application for Avalonia desktop host.
/// Runs on localhost:5000 and serves Blazor components.
/// </summary>
public class ServerAppHost : IDisposable
{
    private WebApplication? app;
    private Task? runTask;
    private bool disposed;

    /// <summary>
    /// Initializes the Blazor Server application with all required services.
    /// </summary>
    public IServiceProvider? ServerServices => app?.Services;

    public async Task StartAsync(IViewLocator avaloniaViewLocator, IAvaloniaWindowHost windowHost)
    {
        if (app != null)
            return;

        try
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = [],
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
            });

            builder.WebHost.UseStaticWebAssets();

            #region Services

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpClient();
            builder.Services.AddLocalization();

            builder.Services

                // Common
                .AddSidekickCommon(SidekickApplicationType.Avalonia)
                .AddSidekickCommonBrowser()
                .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
                .AddSidekickCommonUi()

                // Data
                .AddSidekickData()
                .AddSidekickDataBuilder()

                // Apis
                .AddSidekickGitHubApi()
                .AddSidekickCommonApi()
                .AddSidekickPoeTradeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoe2ScoutApi()
                .AddSidekickPoePriceInfoApi()
                .AddSidekickPoeDbApi()
                .AddSidekickPoeWikiApi()

                // Modules
                .AddSidekickAbout()
                .AddSidekickModuleData()
                .AddSidekickDevelopment()
                .AddSidekickGeneral()
                .AddSidekickItems()
                .AddSidekickLogs()
                .AddSidekickChat()
                .AddSidekickRegexHotkeys()
                .AddSidekickUpdater()
                .AddSidekickWealth();

            builder.Services.AddApexCharts();
            builder.Services.AddSidekickInitializableService<IApplicationService, AvaloniaServerApplicationService>();
            builder.Services.AddSingleton<IViewLocator>(_ => new AvaloniaServerViewLocator(avaloniaViewLocator, windowHost));
            builder.Services.AddSingleton(sp => (AvaloniaServerViewLocator)sp.GetRequiredService<IViewLocator>());

            #endregion Services

            app = builder.Build();

            #region Pipeline

            app.UseStaticFiles();
            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            #endregion Pipeline

            // Start the server without blocking
            runTask = app.RunAsync("http://localhost:5000");

            // Give server a moment to start
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ServerAppHost] Failed to start: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Gracefully stops the Blazor Server application.
    /// </summary>
    public async Task StopAsync()
    {
        if (app == null)
            return;

        try
        {
            await app.StopAsync();
            if (runTask != null)
                await runTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ServerAppHost] Error stopping server: {ex}");
        }
        finally
        {
            app = null;
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;

        try
        {
            StopAsync().Wait(TimeSpan.FromSeconds(5));
        }
        catch
        {
            // Ignore timeout exceptions during disposal
        }
    }
}
