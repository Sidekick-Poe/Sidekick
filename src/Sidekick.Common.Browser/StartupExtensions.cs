using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sidekick.Common.Browser;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonBrowser(this IServiceCollection services)
    {
        services.TryAddSingleton<IBrowserWindowProvider, BrowserWindowProvider>();
        return services;
    }
}
