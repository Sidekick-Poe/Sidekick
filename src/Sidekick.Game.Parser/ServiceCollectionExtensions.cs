using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Game.Languages;
using Sidekick.Game.Texts;

namespace Sidekick.Game;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickGameParser(
        this IServiceCollection services)
    {
        services.AddSidekickInitializableService<GameTextProvider>();

        return services;
    }
}