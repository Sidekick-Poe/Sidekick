using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Initialization;
using Sidekick.Common.Localization;
using Sidekick.Common.Logging;

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
        public static IServiceCollection AddSidekickCommon(this IServiceCollection services)
        {
            services.AddSingleton<ICacheProvider, CacheProvider>();

            // Logging
            var sidekickPath = Environment.ExpandEnvironmentVariables("%AppData%\\sidekick");
            var logSink = new LogSink();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(SidekickPaths.GetDataFilePath("Sidekick_log.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 1,
                    fileSizeLimitBytes: 5242880,
                    rollOnFileSizeLimit: true)
                .WriteTo.Sink(logSink)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });

            services.AddSingleton(logSink);
            services.AddSingleton<IBrowserProvider, BrowserProvider>();

            services.AddSidekickInitializableService<IUILanguageProvider, UILanguageProvider>();

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
    }
}
