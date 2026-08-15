using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Languages;

namespace Sidekick.Game.Providers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickGameProviders(
        this IServiceCollection services)
    {
        services.TryAddSingleton<DataProvider>();
        services.TryAddSingleton<IGameLanguageProvider, GameLanguageProvider>();

        services.AddSidekickInitializableService<BaseItemProvider>();
        services.AddSidekickInitializableService<GameTextProvider>();
        services.AddSidekickInitializableService<ItemClassProvider>();
        services.AddSidekickInitializableService<ItemDefinitionProvider>();

        services.AddSidekickInitializableService<ICurrentGameLanguage, CurrentGameLanguage>();

        return services;
    }
}