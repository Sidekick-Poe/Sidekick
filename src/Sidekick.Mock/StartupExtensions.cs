using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Mock
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickMocks(this IServiceCollection services)
        {
            services.TryAddTransient<IClipboardProvider, MockClipboardProvider>();
            services.TryAddSingleton<IApplicationService, MockApplicationService>();
            services.TryAddSingleton<IProcessProvider, MockProcessProvider>();
            services.TryAddSingleton<IKeyboardProvider, MockKeyboardProvider>();
            services.TryAddSingleton<ITrayProvider, MockTrayProvider>();
            services.TryAddSingleton<IViewLocator, MockViewLocator>();

            return services;
        }
    }
}
