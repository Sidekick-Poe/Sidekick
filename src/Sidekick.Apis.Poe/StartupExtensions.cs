using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Bulk;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Clients.States;
using Sidekick.Apis.Poe.Leagues;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Metadata;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Static;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Common;

namespace Sidekick.Apis.Poe
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeApi(this IServiceCollection services)
        {
            services.AddSingleton<IPoeApiClient, PoeApiClient>();
            services.AddSingleton<IStashService, StashService>();
            services.AddSingleton<IApiStateProvider, ApiStateProvider>();
            services.AddSingleton<PoeApiHandler>();

            services.AddHttpClient(ClientNames.TradeClient);

            services.AddHttpClient(ClientNames.PoeClient)
                .ConfigurePrimaryHttpMessageHandler((sp) =>
                {
                    var authenticationService = sp.GetRequiredService<IAuthenticationService>();
                    var apiStateProvider = sp.GetRequiredService<IApiStateProvider>();
                    var handler = new PoeApiHandler(authenticationService, apiStateProvider);
                    return handler;
                });

            services.AddTransient<IPoeTradeClient, PoeTradeClient>();

            services.AddTransient<FilterResources>();
            services.AddTransient<TradeCurrencyResources>();

            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IItemParser, ItemParser>();
            services.AddSingleton<ITradeSearchService, TradeSearchService>();
            services.AddSingleton<ILeagueProvider, LeagueProvider>();
            services.AddSingleton<ITradeFilterService, TradeFilterService>();
            services.AddSingleton<IBulkTradeService, BulkTradeService>();
            services.AddSingleton<IModifierParser, ModifierParser>();
            services.AddSingleton<ClusterJewelParser>();

            services.AddSidekickInitializableService<IParserPatterns, ParserPatterns>();
            services.AddSidekickInitializableService<SocketParser>();
            services.AddSidekickInitializableService<PropertyParser>();
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
