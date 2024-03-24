using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.GitHub;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickGitHubApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<IGitHubClient, GitHubClient>();

        return services;
    }
}
