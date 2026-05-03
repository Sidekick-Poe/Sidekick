using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sidekick.Apis.PoeDb;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeDbApi(this IServiceCollection services)
    {
        services.TryAddSingleton<IPoeDbClient, PoeDbClient>();

        return services;
    }
}
