using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Common.Cloudflare;
using Sidekick.Apis.Common.States;
using Sidekick.Apis.Poe.Account.Authentication;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Apis.Poe.Account.Stash;
using Sidekick.Common;

namespace Sidekick.Apis.Poe.Account;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeAccountApi(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IPoeApiClient, PoeApiClient>();
        services.AddSingleton<IStashService, StashService>();
        services.AddTransient<PoeApiHandler>();

        services.AddHttpClient(PoeApiClient.ClientName)
            .AddHttpMessageHandler<PoeApiHandler>();

        return services;
    }
}
