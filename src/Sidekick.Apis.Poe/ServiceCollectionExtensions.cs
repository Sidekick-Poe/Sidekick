using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Common;
namespace Sidekick.Apis.Poe;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickPoeApi(
        this IServiceCollection services)
    {
        services.AddSidekickInitializableService<IGameLanguageProvider, GameLanguageProvider>();

        return services;
    }
}
