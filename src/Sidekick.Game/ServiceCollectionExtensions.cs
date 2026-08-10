using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Game.Languages;

namespace Sidekick.Game;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickGame(
        this IServiceCollection services)
    {
        services.AddSingleton<DataProvider>();
        services.TryAddSingleton<IGameLanguageProvider, GameLanguageProvider>();

        return services;
    }
}