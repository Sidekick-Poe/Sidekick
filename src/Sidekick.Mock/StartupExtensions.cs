using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickMocks(this IServiceCollection services)
        {
            services.TryAddSingleton<IAppService, MockAppService>();
            services.TryAddSingleton<IProcessProvider, MockProcessProvider>();
            services.TryAddSingleton<IKeyboardProvider, MockKeyboardProvider>();
            services.TryAddSingleton<IKeybindProvider, MockKeybindProvider>();
            services.TryAddSingleton<IViewLocator, MockViewLocator>();
            services.TryAddScoped<IViewInstance, MockViewInstance>();

            return services;
        }
    }
}
