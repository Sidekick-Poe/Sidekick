using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Poe2Scout.Categories;
using Sidekick.Apis.Poe2Scout.Clients;
using Sidekick.Apis.Poe2Scout.History;
using Sidekick.Apis.Poe2Scout.Items;

namespace Sidekick.Apis.Poe2Scout;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoe2ScoutApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.TryAddSingleton<IScoutClient, ScoutClient>();
        services.TryAddSingleton<IScoutCategoryProvider, ScoutCategoryProvider>();
        services.TryAddSingleton<IScoutItemProvider, ScoutItemProvider>();
        services.TryAddSingleton<IScoutHistoryProvider, ScoutHistoryProvider>();

        return services;
    }
}
