using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.AvaloniaServer;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui;
using Sidekick.Common.Ui.Views;
using Sidekick.Avalonia.Services;
using Sidekick.Common.Database;
using Velopack;

namespace Sidekick.Avalonia;

internal class Program
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;
    public static ServerAppHost? ServerAppHost { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build().Run();

            ServiceProvider = GetServiceProvider();

            // Start the Blazor Server before opening the UI
            var logger = ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogInformation("[Program] Starting Blazor Server...");

            var avaloniaViewLocator = ServiceProvider.GetRequiredService<AvaloniaViewLocator>();
            ServerAppHost = new ServerAppHost();
            ServerAppHost.StartAsync(avaloniaViewLocator, avaloniaViewLocator).Wait(TimeSpan.FromSeconds(5));

            logger?.LogInformation("[Program] Blazor Server started successfully on localhost:5000");

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            var logger = ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogCritical(ex, "[Program] Unhandled exception.");
        }
        finally
        {
            ServerAppHost?.Dispose();
        }
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });

        services.AddLocalization();

        services
            // Common host infrastructure only — all app services live in the Blazor server container
            .AddSidekickCommon(SidekickApplicationType.Avalonia)
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickCommonBrowser()
            .AddSidekickCommonUi()

            // Platform needs to be at the end
            .AddSidekickCommonPlatform(o =>
            {
                o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
                o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
            });

        services.AddSidekickInitializableService<IApplicationService, AvaloniaApplicationService>();
        services.AddSingleton<IViewLocator, AvaloniaViewLocator>();
        services.AddSingleton(sp => (AvaloniaViewLocator)sp.GetRequiredService<IViewLocator>());

        return services.BuildServiceProvider();
    }

}

