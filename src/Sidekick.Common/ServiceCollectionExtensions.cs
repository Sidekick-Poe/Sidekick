using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.GameLogs;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Common.Localization;
using Sidekick.Common.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Common
{
    /// <summary>
    /// Startup functions for the Sidekick project
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds common functionality to the service collection
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <returns>The services collection</returns>
        public static IServiceCollection AddSidekickCommon(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new Settings.Settings();
            configuration.Bind(settings);
            configuration.BindList(nameof(ISettings.Chat_Commands), settings.Chat_Commands);
            services.AddTransient<ISettings>(_ => settings);

            services.AddSingleton<IBrowserProvider, BrowserProvider>();
            services.AddSingleton<ICacheProvider, CacheProvider>();
            services.AddSingleton<IGameLogProvider, GameLogProvider>();

            services.AddSidekickInitializableService<IGameLanguageProvider, GameLanguageProvider>();

            // Validate ClassLanguage implements correct properties
            var properties = typeof(ClassLanguage).GetProperties().Where(x => x.Name != "Prefix");
            foreach (var property in properties)
            {
                if (!Enum.IsDefined(typeof(Class), property.Name))
                {
                    throw new Exception($"ClassLanguage has a property {property.Name} that does not match any Class enum values.");
                }
            }

            services.AddSidekickInitializableService<IUILanguageProvider, UILanguageProvider>();

            return services.AddSidekickLogging();
        }

        private static void BindList<TModel>(this IConfiguration configuration, string key, List<TModel> list)
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

        private static IServiceCollection AddSidekickLogging(this IServiceCollection services)
        {
            var sidekickPath = Environment.ExpandEnvironmentVariables("%AppData%\\sidekick");
            var logSink = new LogSink();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(SidekickPaths.GetDataFilePath("Sidekick_log.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 2,
                    fileSizeLimitBytes: 5242880,
                    rollOnFileSizeLimit: true)
                .WriteTo.Sink(logSink)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });
            services.AddSingleton(logSink);

            return services;
        }

        /// <summary>
        /// Adds a sidekick module to the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assembly">The assembly of the module.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddSidekickModule(this IServiceCollection services, Assembly assembly)
        {
            services.Configure<SidekickConfiguration>(o =>
            {
                o.Modules.Add(assembly);
            });

            return services;
        }

        /// <summary>
        /// Adds an initializable service to the application.
        /// </summary>
        /// <param name="services">The service collection to add an initializable service.</param>
        /// <typeparam name="TService">The type of service to add to the application.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation of the service.</typeparam>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddSidekickInitializableService<TService, TImplementation>(this IServiceCollection services)
            where TService : class, IInitializableService
            where TImplementation : class, TService
        {
            services.AddSingleton<TService, TImplementation>();

            services.Configure<SidekickConfiguration>(o =>
            {
                o.InitializableServices.Add(typeof(TService));
            });

            return services;
        }

        /// <summary>
        /// Adds a keybind to the application
        /// </summary>
        /// <typeparam name="TKeybindHandler">The type of the keybind handler.</typeparam>
        /// <param name="services">The service collection to add the keybind to</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddSidekickKeybind<TKeybindHandler>(this IServiceCollection services)
            where TKeybindHandler : class, IKeybindHandler
        {
            services.AddSingleton<TKeybindHandler>();

            services.Configure<SidekickConfiguration>(o =>
            {
                o.Keybinds.Add(typeof(TKeybindHandler));
            });

            return services;
        }

        /// <summary>
        /// Adds a keybind to the application
        /// </summary>
        /// <typeparam name="TKeybindHandler">The type of the keybind handler.</typeparam>
        /// <param name="services">The service collection to add the keybind to</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddSidekickSettings<TKeybindHandler>(this IServiceCollection services)
            where TKeybindHandler : class, IKeybindHandler
        {
            services.AddSingleton<TKeybindHandler>();

            services.Configure<SidekickConfiguration>(o =>
            {
                o.Keybinds.Add(typeof(TKeybindHandler));
            });

            return services;
        }
    }
}
