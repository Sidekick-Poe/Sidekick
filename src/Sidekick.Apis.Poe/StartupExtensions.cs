using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Bulk;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Leagues;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Metadatas;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Apis.Poe.Static;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Common;

namespace Sidekick.Apis.Poe
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoeTradeClient, PoeTradeClient>();
            services.AddTransient<FilterResources>();
            services.AddTransient<TradeCurrencyResources>();

            services.AddSingleton<IItemParser, ItemParser>();
            services.AddSingleton<ITradeSearchService, TradeSearchService>();
            services.AddSingleton<ILeagueProvider, LeagueProvider>();
            services.AddSingleton<ITradeFilterService, TradeFilterService>();
            services.AddSingleton<IBulkTradeService, BulkTradeService>();
            services.AddSingleton<IModifierParser, ModifierParser>();
            services.AddSingleton<ClusterJewelParser>();

            services.AddSidekickInitializableService<IParserPatterns, ParserPatterns>();
            services.AddSidekickInitializableService<IInvariantMetadataProvider, InvariantMetadataProvider>();
            services.AddSidekickInitializableService<IMetadataProvider, MetadataProvider>();
            services.AddSidekickInitializableService<IItemMetadataParser, MetadataParser>();
            services.AddSidekickInitializableService<IItemStaticDataProvider, ItemStaticDataProvider>();
            services.AddSidekickInitializableService<IInvariantModifierProvider, InvariantModifierProvider>();
            services.AddSidekickInitializableService<IModifierProvider, ModifierProvider>();
            services.AddSidekickInitializableService<IPseudoModifierProvider, PseudoModifierProvider>();

            return services;
        }
    }
}
