using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.Tray;
using Sidekick.Common.Platform.Windows.Keyboards;
using Sidekick.Common.Platform.Windows.Processes;
using Sidekick.Common.Platforms.Localization;

namespace Sidekick.Common.Platform
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services, Action<PlatformOptions> options)
        {
            services.Configure(options);

            services.AddTransient<PlatformResources>();
            services.AddTransient<IClipboardProvider, ClipboardProvider>();
            services.AddSingleton<ITrayProvider, TrayProvider>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.AddSingleton<IProcessProvider, ProcessProvider>();
                services.AddSingleton<IKeyboardProvider, KeyboardProvider>();
            }

            return services;
        }
    }
}
