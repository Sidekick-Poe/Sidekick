using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.PoeDb;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeDbApi(this IServiceCollection services)
    {
        services.AddSingleton<IPoeDbClient, PoeDbClient>();

        return services;
    }
}
