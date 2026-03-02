using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Sidekick.Common;
namespace Sidekick.Modules.Logs;

/// <summary>
/// Startup configuration functions for the general module
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds the general module services to the service collection
    /// </summary>
    /// <param name="services">The services collection to add services to</param>
    /// <returns>The service collection with services added</returns>
    public static IServiceCollection AddSidekickLogs(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddLogging(o =>
        {
            o.AddFilter("Microsoft", LogLevel.Warning);
            o.AddFilter("System", LogLevel.Warning);
            o.AddConsole();
            o.AddSerilog();
        });
        services.AddSingleton(LogHelper.GetLogger("Sidekick_log.log"));

        return services;
    }
}
