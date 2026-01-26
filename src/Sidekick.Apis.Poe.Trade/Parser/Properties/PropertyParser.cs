using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
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
            new ItemClassProperty(game, gameLanguageProvider, serviceProvider, resources),
            new RarityProperty(game, gameLanguageProvider),

            new SeparatorProperty(),

            new QualityProperty(game, gameLanguageProvider),

            new SpiritProperty(game, gameLanguageProvider),
            new ArmourProperty(game, gameLanguageProvider),
            new EvasionRatingProperty(game, gameLanguageProvider),
            new EnergyShieldProperty(game, gameLanguageProvider),
            new BlockChanceProperty(game, gameLanguageProvider),

            new WeaponDamageProperty(game, gameLanguageProvider, resources, invariantStatsProvider),
            new PhysicalDpsProperty(game, resources),
            new ElementalDpsProperty(game, resources),
            new ChaosDpsProperty(game, resources),
            new TotalDpsProperty(game, resources),
            new CriticalHitChanceProperty(game, gameLanguageProvider),
            new AttacksPerSecondProperty(game, gameLanguageProvider),

            new MapTierProperty(game, gameLanguageProvider),
            new RewardProperty(game, gameLanguageProvider, apiItemProvider),
            new RevivesAvailableProperty(game, gameLanguageProvider),
            new MonsterPackSizeProperty(game, gameLanguageProvider),
            new MagicMonstersProperty(game, gameLanguageProvider),
            new RareMonstersProperty(game, gameLanguageProvider),
            new ItemQuantityProperty(game, gameLanguageProvider),
            new ItemRarityProperty(game, gameLanguageProvider),
            new WaystoneDropChanceProperty(game, gameLanguageProvider),
            new AreaLevelProperty(game, gameLanguageProvider),
            new BlightedProperty(game, gameLanguageProvider),
            new BlightRavagedProperty(game, gameLanguageProvider),

            new SeparatorProperty(),

            new GemLevelProperty(game, gameLanguageProvider),
            new ItemLevelProperty(game, gameLanguageProvider),
            new SocketProperty(game, gameLanguageProvider, resources),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.RequirementsCategory?.Title,
                                   new RequiresLevelProperty(game, gameLanguageProvider),
                                   new RequiresStrengthProperty(game, gameLanguageProvider),
                                   new RequiresDexterityProperty(game, gameLanguageProvider),
                                   new RequiresIntelligenceProperty(game, gameLanguageProvider)),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.MiscellaneousCategory?.Title,
                                   new ElderProperty(game, gameLanguageProvider),
                                   new ShaperProperty(game, gameLanguageProvider),
                                   new CrusaderProperty(game, gameLanguageProvider),
                                   new HunterProperty(game, gameLanguageProvider),
                                   new RedeemerProperty(game, gameLanguageProvider),
                                   new WarlordProperty(game, gameLanguageProvider),
                                   new CorruptedProperty(game, gameLanguageProvider),
                                   new FracturedProperty(game, serviceProvider),
                                   new DesecratedProperty(game, serviceProvider),
                                   new SanctifiedProperty(game, serviceProvider),
                                   new MirroredProperty(game, serviceProvider),
                                   new FoulbornProperty(game, serviceProvider),
                                   new UnidentifiedProperty(game, gameLanguageProvider)),

            new ClusterJewelPassiveCountProperty(game, serviceProvider),
        ]);
    }

    public TDefinition GetDefinition<TDefinition>() where TDefinition : PropertyDefinition
    {
        var definition = Definitions.OfType<TDefinition>().FirstOrDefault();
        if (definition != null) return definition;

        definition = Definitions.OfType<ExpandableProperty>().SelectMany(x => x.Definitions).OfType<TDefinition>().FirstOrDefault();
        if (definition != null) return definition;

        throw new SidekickException($"Could not find definition of type {typeof(TDefinition).FullName}");
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
            if (filter == null) continue;

            results.Add(filter);
            await filter.Initialize(item, settingsService);
        }

        return results;
    }
}
