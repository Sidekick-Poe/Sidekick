using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;

namespace Sidekick.Apis.PoeNinja;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IPoeNinjaClient, PoeNinjaClient>();
        services.AddSingleton<INinjaClient, NinjaClient>();
        services.AddSingleton<INinjaExchangeProvider, NinjaExchangeProvider>();

        return services;
    }
}
