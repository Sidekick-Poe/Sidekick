using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Browser;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonBrowser(this IServiceCollection services)
    {
        services.AddSingleton<IBrowserWindowProvider, BrowserWindowProvider>();
        return services;
    }
}
