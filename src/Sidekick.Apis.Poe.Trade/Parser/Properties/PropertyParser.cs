using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class PropertyParser
(
    IServiceProvider serviceProvider,
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider,
    ITradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources,
    IInvariantStatsProvider invariantStatsProvider
) : IPropertyParser
{
    public int Priority => 300;

    private List<PropertyDefinition> Definitions { get; } = new();

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions.Clear();
        Definitions.AddRange([
            new ItemClassProperty(game, serviceProvider),
            new RarityProperty(gameLanguageProvider),

            new SeparatorProperty(),

            new QualityProperty(gameLanguageProvider),

            new SpiritProperty(gameLanguageProvider, game),
            new ArmourProperty(gameLanguageProvider, game),
            new EvasionRatingProperty(gameLanguageProvider, game),
            new EnergyShieldProperty(gameLanguageProvider, game),
            new BlockChanceProperty(gameLanguageProvider, game),

            new WeaponDamageProperty(gameLanguageProvider, game, resources, invariantStatsProvider),
            new CriticalHitChanceProperty(gameLanguageProvider, game),
            new AttacksPerSecondProperty(gameLanguageProvider, game),

            new MapTierProperty(gameLanguageProvider),
            new RewardProperty(gameLanguageProvider, game, apiItemProvider),
            new RevivesAvailableProperty(gameLanguageProvider),
            new MonsterPackSizeProperty(gameLanguageProvider),
            new MagicMonstersProperty(gameLanguageProvider),
            new RareMonstersProperty(gameLanguageProvider),
            new ItemQuantityProperty(gameLanguageProvider),
            new ItemRarityProperty(gameLanguageProvider),
            new WaystoneDropChanceProperty(gameLanguageProvider),
            new AreaLevelProperty(gameLanguageProvider),
            new BlightedProperty(gameLanguageProvider),
            new BlightRavagedProperty(gameLanguageProvider),

            new SeparatorProperty(),

            new GemLevelProperty(gameLanguageProvider),
            new ItemLevelProperty(gameLanguageProvider, game),
            new SocketProperty(gameLanguageProvider, game, resources),

            new SeparatorProperty(),

            new ExpandablePropertiesDefinition(tradeFilterProvider.RequirementsCategory?.Title,
                                               new RequiresLevelProperty(gameLanguageProvider),
                                               new RequiresStrengthProperty(gameLanguageProvider),
                                               new RequiresDexterityProperty(gameLanguageProvider),
                                               new RequiresIntelligenceProperty(gameLanguageProvider)),

            new SeparatorProperty(),

            new ExpandablePropertiesDefinition(tradeFilterProvider.MiscellaneousCategory?.Title,
                                               new ElderProperty(gameLanguageProvider),
                                               new ShaperProperty(gameLanguageProvider),
                                               new CrusaderProperty(gameLanguageProvider),
                                               new HunterProperty(gameLanguageProvider),
                                               new RedeemerProperty(gameLanguageProvider),
                                               new WarlordProperty(gameLanguageProvider),
                                               new CorruptedProperty(gameLanguageProvider),
                                               new FracturedProperty(serviceProvider),
                                               new DesecratedProperty(serviceProvider, game),
                                               new SanctifiedProperty(serviceProvider, game),
                                               new MirroredProperty(serviceProvider),
                                               new FoulbornProperty(serviceProvider, game),
                                               new UnidentifiedProperty(gameLanguageProvider)),

            new ClusterJewelPassiveCountProperty(serviceProvider, game),
        ]);
    }

    public TDefinition GetDefinition<TDefinition>() where TDefinition : PropertyDefinition
    {
        return Definitions.OfType<TDefinition>().FirstOrDefault()
               ?? throw new SidekickException($"Could not find definition of type {typeof(TDefinition).FullName}");
    }

    public void Parse(Item item)
    {
        foreach (var definition in Definitions)
        {
            if (definition.ValidItemClasses.Count > 0 && !definition.ValidItemClasses.Contains(item.Properties.ItemClass)) continue;

            definition.Parse(item);
        }
    }

    public void ParseAfterStats(Item item)
    {
        foreach (var definition in Definitions)
        {
            if (definition.ValidItemClasses.Count > 0 && !definition.ValidItemClasses.Contains(item.Properties.ItemClass)) continue;

            definition.ParseAfterStats(item);
        }
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        var results = new List<TradeFilter>();

        foreach (var definition in Definitions)
        {
            if (definition.ValidItemClasses.Count > 0 && !definition.ValidItemClasses.Contains(item.Properties.ItemClass)) continue;

            var filter = await definition.GetFilter(item);
            if (filter != null) results.Add(filter);

            var filters = definition.GetFilters(item);
            if (filters != null) results.AddRange(filters);
        }

        CleanUpSeparatorFilters(results);

        return results;
    }

    private static void CleanUpSeparatorFilters(List<TradeFilter> results)
    {
        // Remove leading SeparatorProperty filters
        while (results.Count > 0 && results[0] is SeparatorFilter)
        {
            results.RemoveAt(0);
        }

        // Remove trailing SeparatorProperty filters
        while (results.Count > 0 && results[^1] is SeparatorFilter)
        {
            results.RemoveAt(results.Count - 1);
        }

        // Remove consecutive SeparatorProperty filters
        for (var i = 1; i < results.Count; i++)
        {
            if (results[i] is not SeparatorFilter || results[i - 1] is not SeparatorFilter)
            {
                continue;
            }

            results.RemoveAt(i);
            i--;// Adjust index to recheck current position
        }
    }
}
