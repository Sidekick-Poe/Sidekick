using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Update.Localization;

namespace Sidekick.Modules.Update
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickUpdate(this IServiceCollection services)
        {
            services.AddTransient<UpdateResources>();
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            return services;
        }
    }
}
