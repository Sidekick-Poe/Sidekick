using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Stash;
using Sidekick.Apis.PoeNinja.Uris;

namespace Sidekick.Apis.PoeNinja;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<INinjaClient, NinjaClient>();
        services.AddSingleton<INinjaExchangeProvider, NinjaExchangeProvider>();
        services.AddSingleton<INinjaStashProvider, NinjaStashProvider>();
        services.AddSingleton<NinjaUriProvider>();

        return services;
    }
}
