using ApexCharts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
using Sidekick.Web.Components;
using Sidekick.Web.Services;

namespace Sidekick.Web;

public class ServerAppHost(SidekickApplicationType applicationType) : IDisposable
{
    public const int Port = 5000;
    private WebApplication? app;
    private Task? runTask;
    private bool disposed;

    public WebApplication Application =>
        app ?? throw new InvalidOperationException("Server application has not been started.");

    public Task Start(
        Action<IServiceCollection>? configureServices = null,
        CancellationToken cancellationToken = default)
    {
        var assemblyName = typeof(ServerAppHost).Assembly.GetName().Name;
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = assemblyName,
            Args = [],
            ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
        });

        #region Services

        configureServices?.Invoke(builder.Services);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
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
            .AddSidekickWealth()

            // Platform needs to be at the end
            .AddSidekickCommonPlatform();

        builder.Services.AddApexCharts();
        builder.Services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
        builder.Services.TryAddSingleton<IViewLocator, WebViewLocator>();
        builder.Services.TryAddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());

        #endregion Services

        app = builder.Build();

        #region Pipeline

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseAntiforgery();
        app.MapStaticAssets();

        var razorApp = app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        var configuration = app.Services.GetRequiredService<IOptions<SidekickConfiguration>>();
        razorApp.AddAdditionalAssemblies(configuration.Value.Modules.ToArray());

        #endregion Pipeline

        runTask = app.StartAsync(cancellationToken);
        return app.WaitForShutdownAsync(cancellationToken);
    }

    /// <summary>
    /// Gracefully stops the Blazor Server application.
    /// </summary>
    private async Task Stop()
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
            Stop().Wait(TimeSpan.FromSeconds(5));
        }
        catch
        {
            // Ignore timeout exceptions during disposal
        }
    }
}