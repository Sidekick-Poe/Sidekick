using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Platform.Interprocess;

namespace Sidekick.Common.Interprocess;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonInterprocess(this IServiceCollection services)
    {
        services.AddSingleton<IInterprocessService, InterprocessService>();

        return services;
    }
}
