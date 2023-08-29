using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Game.GameLogs;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Common.Game
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickCommonGame(this IServiceCollection services)
        {
            services.AddSingleton<IGameLogProvider, GameLogProvider>();

            services.AddSidekickInitializableService<IGameLanguageProvider, GameLanguageProvider>();

            // Validate ClassLanguage implements correct properties
            var properties = typeof(ClassLanguage).GetProperties().Where(x => x.Name != "Prefix");
            foreach (var property in properties)
            {
                if (!Enum.IsDefined(typeof(Class), property.Name))
                {
                    throw new Exception($"ClassLanguage has a property {property.Name} that does not match any Class enum values.");
                }
            }

            return services;
        }
    }
}
