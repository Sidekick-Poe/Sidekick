using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Leagues;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Metadatas;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Apis.Poe.Static;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Translations.Stats;

namespace Sidekick.Apis.Poe
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoeTradeClient, PoeTradeClient>();
            services.AddTransient<FilterResources>();

            services.AddSingleton<IItemParser, ItemParser>();
            services.AddSingleton<IItemMetadataProvider, ItemMetadataProvider>();
            services.AddSingleton<IModifierProvider, ModifierProvider>();
            services.AddSingleton<IEnglishModifierProvider, EnglishModifierProvider>();
            services.AddSingleton<IPseudoModifierProvider, PseudoModifierProvider>();
            services.AddSingleton<IItemStaticDataProvider, ItemStaticDataProvider>();
            services.AddSingleton<IStatTranslationProvider, StatTranslationProvider>();
            services.AddSingleton<IParserPatterns, ParserPatterns>();
            services.AddSingleton<ITradeSearchService, TradeSearchService>();
            services.AddSingleton<ILeagueProvider, LeagueProvider>();
            services.AddSingleton<ITradeFilterService, TradeFilterService>();

            return services;
        }
    }
}
