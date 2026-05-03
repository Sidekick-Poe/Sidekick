using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sidekick.Apis.PoePriceInfo;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoePriceInfoApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.TryAddSingleton<IPoePriceInfoClient, PoePriceInfoClient>();

        return services;
    }
}
