using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Blazor.Errors;
using Sidekick.Common.Blazor.Initialization;
using Sidekick.Common.Blazor.Update;

namespace Sidekick.Common.Blazor
{
    /// <summary>
    /// Extensions for the service collection interface for setup code.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the sidekick blazor functionality to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddSidekickCommonBlazor(this IServiceCollection services)
        {
            services.AddTransient<ErrorResources>();
            services.AddTransient<InitializationResources>();
            services.AddTransient<UpdateResources>();

            return services;
        }
    }
}
