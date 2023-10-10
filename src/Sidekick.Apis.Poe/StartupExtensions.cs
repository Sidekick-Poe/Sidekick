using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Authentication;
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
using Sidekick.Common;

namespace Sidekick.Apis.Poe
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeApi(this IServiceCollection services)
        {
            services.AddSingleton<PoeApiClient>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<PoeApiHandler>();

            services.AddHttpClient("PoeTradeClient");

            services.AddHttpClient("PoeClient")
                .ConfigurePrimaryHttpMessageHandler((sp) =>
                {
                    var logger = sp.GetRequiredService<ILogger<PoeApiHandler>>();
                    var authenticationService = sp.GetRequiredService<IAuthenticationService>();
                    var handler = new PoeApiHandler(logger, authenticationService);
                    return handler;
                });

            services.AddTransient<IPoeTradeClient, PoeTradeClient>();

            services.AddTransient<FilterResources>();
            services.AddTransient<TradeCurrencyResources>();

            services.AddSingleton<IItemParser, ItemParser>();
            services.AddSingleton<ITradeSearchService, TradeSearchService>();
            services.AddSingleton<ILeagueProvider, LeagueProvider>();
            services.AddSingleton<ITradeFilterService, TradeFilterService>();

            services.AddSidekickInitializableService<IParserPatterns, ParserPatterns>();
            services.AddSidekickInitializableService<IItemMetadataProvider, ItemMetadataProvider>();
            services.AddSidekickInitializableService<IItemStaticDataProvider, ItemStaticDataProvider>();
            services.AddSidekickInitializableService<IEnglishModifierProvider, EnglishModifierProvider>();
            services.AddSidekickInitializableService<IModifierProvider, ModifierProvider>();
            services.AddSidekickInitializableService<IPseudoModifierProvider, PseudoModifierProvider>();

            return services;
        }
    }
}
