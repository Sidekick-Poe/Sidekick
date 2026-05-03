using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Web;
using Velopack;

namespace Sidekick.Avalonia;

internal class Program
{
    public static IServiceProvider ServiceProvider =>
        ServerAppHost?.Application.Services ?? throw new Exception("ServerAppHost is null");

    private static ServerAppHost? ServerAppHost { get; set; }

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build().Run();

            // Start the Blazor Server before opening the UI
            ServerAppHost = new ServerAppHost(SidekickApplicationType.Avalonia);
            ServerAppHost
                .StartAsync(
                    url: "http://localhost:5000",
                    configureServices: services =>
                    {
                        services.AddLogging(builder => { builder.AddConsole(); });
                    })
                .Wait(TimeSpan.FromSeconds(5));

            var logger = ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogInformation("[Program] Blazor Server started successfully on localhost:5000");

            AppBuilder.Configure<App>()
                .UsePlatformDetect()
#if DEBUG
                .WithDeveloperTools()
#endif
                .WithInterFont()
                .LogToTrace()
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
}