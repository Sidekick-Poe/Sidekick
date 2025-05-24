using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Common.Cloudflare;
using Sidekick.Apis.Common.Limiter;
using Sidekick.Apis.Common.States;

namespace Sidekick.Apis.Common;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonApi(this IServiceCollection services)
    {
        services.AddSingleton<ICloudflareService, CloudflareService>();
        services.AddSingleton<IApiStateProvider, ApiStateProvider>();
        services.AddSingleton<ApiLimiterProvider>();

        return services;
    }
}
