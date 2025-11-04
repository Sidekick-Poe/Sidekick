using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Items;
using Sidekick.Apis.PoeNinja.Stash;
using Sidekick.Common;

namespace Sidekick.Apis.PoeNinja;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<INinjaClient, NinjaClient>();
        services.AddSingleton<INinjaExchangeProvider, NinjaExchangeProvider>();
        services.AddSingleton<INinjaStashProvider, NinjaStashProvider>();
        services.AddSingleton<INinjaPageProvider, NinjaPageProvider>();
        services.AddSidekickInitializableService<INinjaItemProvider, NinjaItemProvider>();

        return services;
    }
}
