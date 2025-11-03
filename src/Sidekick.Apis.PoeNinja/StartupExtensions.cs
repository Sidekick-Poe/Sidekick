using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Items;
using Sidekick.Common;

namespace Sidekick.Apis.PoeNinja;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IPoeNinjaClient, PoeNinjaClient>();
        services.AddSingleton<INinjaClient, NinjaClient>();
        services.AddSingleton<INinjaExchangeProvider, NinjaExchangeProvider>();
        services.AddSingleton<INinjaPageProvider, NinjaPageProvider>();
        services.AddSidekickInitializableService<INinjaItemProvider, NinjaItemProvider>();

        return services;
    }
}
