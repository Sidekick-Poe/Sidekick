using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.Keybinds;
using Sidekick.Common.Platform.Keyboards;
using Sidekick.Common.Platform.Options;
using Sidekick.Common.Platform.Windows.Processes;
using Sidekick.Common.Platforms.Localization;

namespace Sidekick.Common.Platform
{
    /// <summary>
    /// Functions for startup configuration for platform related features
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Adds platform (operating system) functions to the service collection
        /// </summary>
        /// <param name="services">The services collection to add services to</param>
        /// <returns>The service collection with services added</returns>
        public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services, Action<PlatformOptions> options)
        {
            services.Configure(options);

            services.AddTransient<PlatformResources>();
            services.AddTransient<IClipboardProvider, ClipboardProvider>();
            services.AddSingleton<IKeybindProvider, KeybindProvider>();
            services.AddSingleton<IKeyboardProvider, KeyboardProvider>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.AddSingleton<IProcessProvider, ProcessProvider>();
            }

            return services;
        }

        /// <summary>
        /// Adds a keybind to the application
        /// </summary>
        /// <param name="services">The service collection to add the keybind to</param>
        /// <param name="keybindHandler">The keybind handler to add</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddSidekickKeybind<TKeybindHandler>(this IServiceCollection services)
            where TKeybindHandler : class, IKeybindHandler
        {
            services.AddSingleton<TKeybindHandler>();

            services.Configure<KeybindOptions>(o =>
            {
                o.Keybinds.Add(typeof(TKeybindHandler));
            });

            return services;
        }
    }
}
