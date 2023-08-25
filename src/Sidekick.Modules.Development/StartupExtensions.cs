using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.Development
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickDevelopment(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            return services;
        }
    }
}
