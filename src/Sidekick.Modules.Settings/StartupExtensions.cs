using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Common.Settings;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickSettings(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddTransient<SettingsResources>();

            return services;
        }
    }
}
