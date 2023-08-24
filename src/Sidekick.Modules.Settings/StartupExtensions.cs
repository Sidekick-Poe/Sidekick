using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Common.Settings;
using Sidekick.Modules.Settings.Localization;

namespace Sidekick.Modules.Settings
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddTransient<SettingsResources>();
            services.AddTransient<SetupResources>();

            services.AddScoped<SettingsModel>();

            services.AddSingleton<ISettingsService, SettingsService>();

            var settings = new Settings();
            configuration.Bind(settings);
            configuration.BindList(nameof(ISettings.Chat_Commands), settings.Chat_Commands);
            configuration.BindList(nameof(ISettings.Cheatsheets_Pages), settings.Cheatsheets_Pages);
            services.AddSingleton(settings);
            services.AddSingleton<ISettings>(sp => sp.GetRequiredService<Settings>());

            return services;
        }

        public static void BindList<TModel>(this IConfiguration configuration, string key, List<TModel> list)
            where TModel : new()
        {
            var items = configuration.GetSection(key).GetChildren().ToList();
            if (items.Count > 0)
            {
                list.Clear();
            }
            foreach (var item in items)
            {
                var model = new TModel();
                item.Bind(model);
                list.Add(model);
            }
        }
    }
}
