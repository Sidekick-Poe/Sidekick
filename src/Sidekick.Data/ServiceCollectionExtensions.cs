using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sidekick.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickData(
        this IServiceCollection services)
    {
        services.TryAddSingleton<DataProvider>();

        return services;
    }
}