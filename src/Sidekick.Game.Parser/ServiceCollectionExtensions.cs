using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Game.Parser.Texts;
namespace Sidekick.Game.Parser;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickGameParser(
        this IServiceCollection services)
    {
        services.AddSidekickInitializableService<GameTextProvider>();

        return services;
    }
}