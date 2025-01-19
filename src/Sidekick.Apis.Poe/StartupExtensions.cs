using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Bulk;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Clients.States;
using Sidekick.Apis.Poe.CloudFlare;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Leagues;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Parser.Pseudo;
using Sidekick.Apis.Poe.Parser.Requirements;
using Sidekick.Apis.Poe.Parser.Sockets;
using Sidekick.Apis.Poe.Stash;
using Sidekick.Apis.Poe.Static;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Common;

namespace Sidekick.Apis.Poe;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickPoeApi(this IServiceCollection services)
    {
        services.AddSingleton<IPoeApiClient, PoeApiClient>();
        services.AddSingleton<IStashService, StashService>();
        services.AddSingleton<IApiStateProvider, ApiStateProvider>();
        services.AddTransient<PoeApiHandler>();
        services.AddTransient<PoeTradeHandler>();

        services.AddHttpClient(ClientNames.TradeClient)
            .AddHttpMessageHandler<PoeTradeHandler>();

        services.AddHttpClient(ClientNames.PoeClient)
            .AddHttpMessageHandler<PoeApiHandler>();

        services.AddTransient<IPoeTradeClient, PoeTradeClient>();

        services.AddTransient<FilterResources>();

        services.AddSingleton<ICloudflareService, CloudflareService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IItemParser, ItemParser>();
        services.AddSingleton<ITradeSearchService, TradeSearchService>();
        services.AddSingleton<ILeagueProvider, LeagueProvider>();
        services.AddSingleton<ITradeFilterService, TradeFilterService>();
        services.AddSingleton<IBulkTradeService, BulkTradeService>();
        services.AddSingleton<IModifierParser, ModifierParser>();
        services.AddSingleton<ClusterJewelParser>();
        services.AddSingleton<IFuzzyService, FuzzyService>();

        services.AddSidekickInitializableService<IRequirementsParser, RequirementsParser>();
        services.AddSidekickInitializableService<ISocketParser, SocketParser>();
        services.AddSidekickInitializableService<IPropertyParser, PropertyParser>();
        services.AddSidekickInitializableService<IApiInvariantItemProvider, ApiInvariantItemProvider>();
        services.AddSidekickInitializableService<IApiItemProvider, ApiItemProvider>();
        services.AddSidekickInitializableService<IItemStaticDataProvider, ItemStaticDataProvider>();
        services.AddSidekickInitializableService<IInvariantModifierProvider, InvariantModifierProvider>();
        services.AddSidekickInitializableService<IModifierProvider, ModifierProvider>();
        services.AddSidekickInitializableService<IPseudoParser, PseudoParser>();
        services.AddSidekickInitializableService<IFilterProvider, FilterProvider>();
        services.AddSidekickInitializableService<IHeaderParser, HeaderParser>();

        return services;
    }
}
