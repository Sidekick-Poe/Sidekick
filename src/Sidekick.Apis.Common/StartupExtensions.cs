using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Apis.Common.Cloudflare;
using Sidekick.Apis.Common.Limiter;
using Sidekick.Apis.Common.States;

namespace Sidekick.Apis.Common;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonApi(this IServiceCollection services)
    {
        services.TryAddSingleton<ICloudflareService, CloudflareService>();
        services.TryAddSingleton<IApiStateProvider, ApiStateProvider>();
        services.TryAddSingleton<ApiLimiterProvider>();

        return services;
    }
}
