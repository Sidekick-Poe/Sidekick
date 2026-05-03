using ApexCharts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Common;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe.Account;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe2Scout;
using Sidekick.Apis.PoeDb;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
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
using Sidekick.Web.Services;

namespace Sidekick.Web;

/// <summary>
/// Manages the embedded Blazor Server application for Avalonia desktop host.
/// Runs on localhost:5000 and serves Blazor components.
/// </summary>
public class ServerAppHost(SidekickApplicationType applicationType) : IDisposable
{
    private WebApplication? app;
    private Task? runTask;
    private bool disposed;

    public WebApplication Application => app ?? throw new InvalidOperationException("Server application has not been started.");

    public Task StartAsync(
        string? url = null,
        Action<IServiceCollection>? configureServices = null)
    {
        if (app != null) return Task.CompletedTask;

        try
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = [],
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
            });

            builder.WebHost.UseStaticWebAssets();

            #region Services

            configureServices?.Invoke(builder.Services);

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpClient();
            builder.Services.AddLocalization();

            builder.Services

                // Common
                .AddSidekickCommon(applicationType)
                .AddSidekickCommonBrowser()
                .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
                .AddSidekickCommonUi()

                // Data
                .AddSidekickData()
                .AddSidekickDataBuilder()

                // Apis
                .AddSidekickGitHubApi()
                .AddSidekickCommonApi()
                .AddSidekickPoeAccountApi()
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
            builder.Services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
            builder.Services.TryAddSingleton<IViewLocator, WebViewLocator>();
            builder.Services.TryAddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());

            #endregion Services

            app = builder.Build();

            #region Pipeline

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseStaticFiles();
            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            #endregion Pipeline

            // Start the server without blocking
            runTask = app.RunAsync(url);
            return runTask;
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
    private async Task StopAsync()
    {
        if (app == null) return;

        try
        {
            await app.StopAsync();
            if (runTask != null) await runTask;
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
        if (disposed) return;

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
