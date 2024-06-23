using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickSettings(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddTransient<SettingsResources>();
            services.AddTransient<SetupResources>();

            services.AddScoped<SettingsModel>();

            return services;
        }
    }
}
