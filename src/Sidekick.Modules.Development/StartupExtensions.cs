using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Blazor;

namespace Sidekick.Modules.Development
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickDevelopment(this IServiceCollection services)
        {
            services.AddSidekickModule(new SidekickModule()
            {
                Assembly = typeof(StartupExtensions).Assembly
            });

            return services;
        }
    }
}
