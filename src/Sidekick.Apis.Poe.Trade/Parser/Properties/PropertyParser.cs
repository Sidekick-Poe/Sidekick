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
            new ItemClassProperty(game, settingsService, gameLanguageProvider, serviceProvider),
            new RarityProperty(game, settingsService, gameLanguageProvider),

            new SeparatorProperty(),

            new QualityProperty(game, settingsService, gameLanguageProvider),

            new SpiritProperty(game, settingsService, gameLanguageProvider),
            new ArmourProperty(game, settingsService, gameLanguageProvider),
            new EvasionRatingProperty(game, settingsService, gameLanguageProvider),
            new EnergyShieldProperty(game, settingsService, gameLanguageProvider),
            new BlockChanceProperty(game, settingsService, gameLanguageProvider),

            new WeaponDamageProperty(game, settingsService, gameLanguageProvider, resources, invariantStatsProvider),
            new PhysicalDpsProperty(game, settingsService, gameLanguageProvider, resources),
            new ElementalDpsProperty(game, settingsService, gameLanguageProvider, resources),
            new ChaosDpsProperty(game, settingsService, gameLanguageProvider, resources),
            new TotalDpsProperty(game, settingsService, gameLanguageProvider, resources),
            new CriticalHitChanceProperty(game, settingsService, gameLanguageProvider),
            new AttacksPerSecondProperty(game, settingsService, gameLanguageProvider),

            new MapTierProperty(game, settingsService, gameLanguageProvider),
            new RewardProperty(game, settingsService, gameLanguageProvider, apiItemProvider),
            new RevivesAvailableProperty(game, settingsService, gameLanguageProvider),
            new MonsterPackSizeProperty(game, settingsService, gameLanguageProvider),
            new MagicMonstersProperty(game, settingsService, gameLanguageProvider),
            new RareMonstersProperty(game, settingsService, gameLanguageProvider),
            new ItemQuantityProperty(game, settingsService, gameLanguageProvider),
            new ItemRarityProperty(game, settingsService, gameLanguageProvider),
            new WaystoneDropChanceProperty(game, settingsService, gameLanguageProvider),
            new AreaLevelProperty(game, settingsService, gameLanguageProvider),
            new BlightedProperty(game, settingsService, gameLanguageProvider),
            new BlightRavagedProperty(game, settingsService, gameLanguageProvider),

            new SeparatorProperty(),

            new GemLevelProperty(game, settingsService, gameLanguageProvider),
            new ItemLevelProperty(game, settingsService, gameLanguageProvider),
            new SocketProperty(game, settingsService, gameLanguageProvider, resources),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.RequirementsCategory?.Title,
                                               new RequiresLevelProperty(game, settingsService, gameLanguageProvider),
                                               new RequiresStrengthProperty(game, settingsService, gameLanguageProvider),
                                               new RequiresDexterityProperty(game, settingsService, gameLanguageProvider),
                                               new RequiresIntelligenceProperty(game, settingsService, gameLanguageProvider)),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.MiscellaneousCategory?.Title,
                                               new ElderProperty(game, settingsService, gameLanguageProvider),
                                               new ShaperProperty(game, settingsService, gameLanguageProvider),
                                               new CrusaderProperty(game, settingsService, gameLanguageProvider),
                                               new HunterProperty(game, settingsService, gameLanguageProvider),
                                               new RedeemerProperty(game, settingsService, gameLanguageProvider),
                                               new WarlordProperty(game, settingsService, gameLanguageProvider),
                                               new CorruptedProperty(game, settingsService, gameLanguageProvider),
                                               new FracturedProperty(game, settingsService, gameLanguageProvider, serviceProvider),
                                               new DesecratedProperty(game, settingsService, gameLanguageProvider, serviceProvider),
                                               new SanctifiedProperty(game, settingsService, gameLanguageProvider, serviceProvider),
                                               new MirroredProperty(game, settingsService, gameLanguageProvider, serviceProvider),
                                               new FoulbornProperty(game, settingsService, gameLanguageProvider, serviceProvider),
                                               new UnidentifiedProperty(game, settingsService, gameLanguageProvider)),

            new ClusterJewelPassiveCountProperty(game, settingsService, gameLanguageProvider, serviceProvider),
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
        }

        return results;
    }
}
