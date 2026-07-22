using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Poe.Account.Authentication;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Apis.Poe.Account.Stash;

namespace Sidekick.Apis.Poe.Account;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeAccountApi(this IServiceCollection services)
    {
        services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
        services.TryAddSingleton<IAccountApiClient, AccountApiClient>();
        services.TryAddSingleton<IStashService, StashService>();
        services.TryAddTransient<AccountApiHandler>();

        services.AddHttpClient(AccountApiClient.ClientName)
            .AddHttpMessageHandler<AccountApiHandler>();

        return services;
    }
}
