using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Properties.Definitions;
using Sidekick.Game.Providers;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class PropertyParser
(
    IServiceProvider serviceProvider,
    ICurrentGameLanguage currentGameLanguage,
    ItemDefinitionProvider itemDefinitionProvider,
    TradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService,
    GameTextProvider gameTextProvider,
    IStringLocalizer<PoeResources> resources) : IInitializableService
{
    private List<PropertyDefinition> Definitions { get; } = new();

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions.Clear();
        Definitions.AddRange([
            new ItemClassProperty(game, gameTextProvider),
            new RarityProperty(game, gameTextProvider),

            new SeparatorProperty(),

            new QualityProperty(game, currentGameLanguage),

            new SpiritProperty(game, currentGameLanguage),
            new ArmourProperty(game, gameTextProvider),
            new EvasionRatingProperty(game, gameTextProvider),
            new EnergyShieldProperty(game, gameTextProvider),
            new BlockChanceProperty(game, gameTextProvider),

            new WeaponDamageProperty(game, currentGameLanguage, serviceProvider, tradeFilterProvider),
            new PhysicalDpsProperty(game, gameTextProvider),
            new ElementalDpsProperty(game, gameTextProvider),
            new ChaosDpsProperty(game, resources),
            new TotalDpsProperty(game, gameTextProvider),
            new CriticalHitChanceProperty(game, gameTextProvider),
            new AttacksPerSecondProperty(game, gameTextProvider),
            new MemoryStrandsProperty(game, currentGameLanguage),

            new BlightedProperty(game, gameTextProvider),
            new BlightRavagedProperty(game, gameTextProvider),
            new MapTierProperty(game, currentGameLanguage),
            new RewardProperty(game, currentGameLanguage, itemDefinitionProvider),
            new RevivesAvailableProperty(game, currentGameLanguage),
            new MonsterPackSizeProperty(game, currentGameLanguage),

            new MagicMonstersProperty(game, currentGameLanguage),
            new RareMonstersProperty(game, currentGameLanguage),
            new ItemQuantityProperty(game, currentGameLanguage),
            new ItemRarityProperty(game, currentGameLanguage),
            new MoreMapsProperty(game, currentGameLanguage),
            new MoreScarabsProperty(game, currentGameLanguage),
            new MoreCurrencyProperty(game, currentGameLanguage),
            new MoreCardsProperty(game, currentGameLanguage),
            new QualityCurrencyProperty(game, currentGameLanguage),
            new QualityScarabsProperty(game, currentGameLanguage),
            new QualityCardsProperty(game, currentGameLanguage),
            new QualityPackSizeProperty(game, currentGameLanguage),
            new QualityRarityProperty(game, currentGameLanguage),
            new WaystoneDropChanceProperty(game, currentGameLanguage),
            new AreaLevelProperty(game, currentGameLanguage),

            new HeistWingsRevealedProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistWingsTotalProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistRoutesRevealedProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistRoutesTotalProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistRoomsRevealedProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistRoomsTotalProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistObjectiveValueProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistLockpickingProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistDemolitionProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistAgilityProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistCounterThaumaturgyProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistTrapDisarmamentProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistPerceptionProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistBruteForceProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistDeceptionProperty(game, currentGameLanguage, tradeFilterProvider),
            new HeistEngineeringProperty(game, currentGameLanguage, tradeFilterProvider),

            new SeparatorProperty(),

            new GemLevelProperty(game, currentGameLanguage),
            new ItemLevelProperty(game, currentGameLanguage),
            new SocketCountProperty(game, currentGameLanguage),
            new SocketLinkProperty(game, resources),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.RequirementsCategory?.Title,
                                   new RequiresLevelProperty(game, currentGameLanguage),
                                   new RequiresStrengthProperty(game, currentGameLanguage),
                                   new RequiresDexterityProperty(game, currentGameLanguage),
                                   new RequiresIntelligenceProperty(game, currentGameLanguage)),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.MiscellaneousCategory?.Title,
                                   new ElderProperty(game, gameTextProvider),
                                   new ShaperProperty(game, gameTextProvider),
                                   new CrusaderProperty(game, gameTextProvider),
                                   new HunterProperty(game, gameTextProvider),
                                   new RedeemerProperty(game, gameTextProvider),
                                   new WarlordProperty(game, gameTextProvider),
                                   new CorruptedProperty(game, currentGameLanguage),
                                   new SplitProperty(game, currentGameLanguage),
                                   new FracturedProperty(game, tradeFilterProvider),
                                   new DesecratedProperty(game, tradeFilterProvider),
                                   new SanctifiedProperty(game, tradeFilterProvider),
                                   new MirroredProperty(game, currentGameLanguage),
                                   new FoulbornProperty(game, tradeFilterProvider),
                                   new ImbuedGemProperty(game, tradeFilterProvider),
                                   new UnidentifiedProperty(game, currentGameLanguage)),
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
            definition.Parse(item);
        }
    }

    public void ParseAfterStats(Item item)
    {
        foreach (var definition in Definitions)
        {
            definition.ParseAfterStats(item);
        }
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        var results = new List<TradeFilter>();

        foreach (var definition in Definitions)
        {
            var filter = await definition.GetFilter(item);
            if (filter == null) continue;

            results.Add(filter);
            await filter.Initialize(item, settingsService);
        }

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

        return results;
    }
}
