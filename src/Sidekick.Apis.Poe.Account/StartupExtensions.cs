using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Account.Authentication;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Apis.Poe.Account.Stash;

namespace Sidekick.Apis.Poe.Account;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeAccountApi(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IAccountApiClient, AccountApiClient>();
        services.AddSingleton<IStashService, StashService>();
        services.AddTransient<AccountApiHandler>();

        services.AddHttpClient(AccountApiClient.ClientName)
            .AddHttpMessageHandler<AccountApiHandler>();

        return services;
    }
}
