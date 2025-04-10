using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Platform.Interprocess;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonInterprocess(this IServiceCollection services)
    {
        services.AddSingleton<IInterprocessService, InterprocessService>();

        return services;
    }
}
