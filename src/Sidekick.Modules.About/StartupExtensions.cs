using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.About.Localization;

namespace Sidekick.Modules.About
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickAbout(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddTransient<AboutResources>();

            return services;
        }
    }
}
