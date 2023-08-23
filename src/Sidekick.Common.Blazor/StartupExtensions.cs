using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Blazor.Errors;

namespace Sidekick.Common.Blazor
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickModule(this IServiceCollection services, SidekickModule module)
        {
            SidekickModule.Modules.Add(module);
            services.AddTransient<ErrorResources>();

            return services;
        }
    }
}
