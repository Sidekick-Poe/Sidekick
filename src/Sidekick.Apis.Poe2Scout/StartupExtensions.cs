using Microsoft.Extensions.DependencyInjection;
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
        services.AddSingleton<IScoutClient, ScoutClient>();
        services.AddSingleton<IScoutCategoryProvider, ScoutCategoryProvider>();
        services.AddSingleton<IScoutItemProvider, ScoutItemProvider>();
        services.AddSingleton<IScoutHistoryProvider, ScoutHistoryProvider>();

        return services;
    }
}
