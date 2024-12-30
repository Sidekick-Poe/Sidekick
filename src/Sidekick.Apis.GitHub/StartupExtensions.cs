using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.GitHub;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickGitHubApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IGitHubClient, GitHubClient>();

        return services;
    }
}
