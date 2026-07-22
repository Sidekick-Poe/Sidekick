using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services.TryAddSingleton<INinjaClient, NinjaClient>();
        services.TryAddSingleton<INinjaExchangeProvider, NinjaExchangeProvider>();
        services.TryAddSingleton<INinjaStashProvider, NinjaStashProvider>();
        services.TryAddSingleton<NinjaUriProvider>();

        return services;
    }
}
