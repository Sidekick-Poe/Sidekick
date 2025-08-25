using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.Development;

/// <summary>
/// Startup configuration functions for the development module
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds the development module services to the service collection
    /// </summary>
    /// <param name="services">The services collection to add services to</param>
    /// <returns>The service collection with services added</returns>
    public static IServiceCollection AddSidekickDevelopment(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        return services;
    }
}
