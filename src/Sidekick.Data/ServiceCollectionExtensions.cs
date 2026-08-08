using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Data.Languages;
using Sidekick.Data.Texts;

namespace Sidekick.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickData(
        this IServiceCollection services)
    {
        services.AddSidekickInitializableService<ICurrentGameLanguage, CurrentGameLanguage>();
        services.AddSidekickInitializableService<GameTextProvider>();

        services.AddSingleton<DataProvider>();
        services.TryAddSingleton<IGameLanguageProvider, GameLanguageProvider>();

        return services;
    }
}