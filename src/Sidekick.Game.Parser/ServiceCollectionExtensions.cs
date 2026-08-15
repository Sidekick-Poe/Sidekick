using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Game.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickGameParser(
        this IServiceCollection services)
    {
        return services;
    }
}