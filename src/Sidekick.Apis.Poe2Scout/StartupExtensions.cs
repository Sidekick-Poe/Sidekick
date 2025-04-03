using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.Poe2Scout;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoe2ScoutApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IPoe2ScoutClient, Poe2ScoutClient>();

        return services;
    }
}
