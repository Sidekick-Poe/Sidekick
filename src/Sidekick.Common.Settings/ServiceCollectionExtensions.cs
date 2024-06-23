using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickSettings(
        this IServiceCollection services)
    {
        services.AddSingleton<ISettingsService, SettingsService>();
        return services;
    }
}
