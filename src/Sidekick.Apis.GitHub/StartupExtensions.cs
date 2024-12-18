using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Apis.GitHub;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickGitHubApi(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSidekickInitializableService<IGitHubClient, GitHubClient>();

        return services;
    }
}
