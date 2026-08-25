using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Poe2Scout.History;
using Sidekick.Apis.Poe2Scout.Urls;

namespace Sidekick.Apis.Poe2Scout;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoe2ScoutApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.TryAddSingleton<ScoutHistoryProvider>();
        services.TryAddSingleton<ScoutUrlProvider>();

        return services;
    }
}
