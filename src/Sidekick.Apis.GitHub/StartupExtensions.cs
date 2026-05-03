using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sidekick.Apis.GitHub;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickGitHubApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.TryAddSingleton<IGitHubClient, GitHubClient>();

        return services;
    }
}
