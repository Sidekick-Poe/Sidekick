using Microsoft.Extensions.Localization;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Localization;
using Sidekick.Game.Parser.Properties.Definitions;
using Sidekick.Game.Providers;
using Sidekick.Game.Providers.Languages;
namespace Sidekick.Game.Parser.Properties;

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

            new QualityProperty(game, gameTextProvider),

            new SpiritProperty(game, gameTextProvider),
            new ArmourProperty(game, gameTextProvider),
            new EvasionRatingProperty(game, gameTextProvider),
            new EnergyShieldProperty(game, gameTextProvider),
            new RunicWardProperty(game, gameTextProvider),
            new BlockChanceProperty(game, gameTextProvider),

            new WeaponDamageProperty(game, gameTextProvider, serviceProvider, tradeFilterProvider),
            new PhysicalDpsProperty(game, gameTextProvider),
            new ElementalDpsProperty(game, gameTextProvider),
            new ChaosDpsProperty(game, resources),
            new TotalDpsProperty(game, gameTextProvider),
            new CriticalHitChanceProperty(game, gameTextProvider),
            new AttacksPerSecondProperty(game, gameTextProvider),
            new MemoryStrandsProperty(game, gameTextProvider),

            new BlightedProperty(game, gameTextProvider),
            new BlightRavagedProperty(game, gameTextProvider),
            new MapTierProperty(game, gameTextProvider),
            new RewardProperty(game, gameTextProvider, itemDefinitionProvider),
            new RevivesAvailableProperty(game, gameTextProvider),
            new MonsterPackSizeProperty(game, gameTextProvider),
            new MagicMonstersProperty(game, gameTextProvider),
            new RareMonstersProperty(game, gameTextProvider),
            new ItemQuantityProperty(game, gameTextProvider),
            new ItemRarityProperty(game, gameTextProvider),
            new MonsterEffectivenessProperty(game, gameTextProvider),
            new MoreMapsProperty(game, gameTextProvider),
            new MoreScarabsProperty(game, gameTextProvider),
            new MoreCurrencyProperty(game, gameTextProvider),
            new MoreCardsProperty(game, gameTextProvider),
            new QualityCurrencyProperty(game, currentGameLanguage),
            new QualityScarabsProperty(game, currentGameLanguage),
            new QualityCardsProperty(game, currentGameLanguage),
            new QualityPackSizeProperty(game, currentGameLanguage),
            new QualityRarityProperty(game, currentGameLanguage),
            new WaystoneDropChanceProperty(game, gameTextProvider),
            new AreaLevelProperty(game, gameTextProvider),

            new HeistWingsRevealedProperty(game, gameTextProvider, tradeFilterProvider),
            new HeistWingsTotalProperty(game, gameTextProvider, tradeFilterProvider),
            new HeistRoutesRevealedProperty(game, gameTextProvider, tradeFilterProvider),
            new HeistRoutesTotalProperty(game, gameTextProvider, tradeFilterProvider),
            new HeistRoomsRevealedProperty(game, gameTextProvider, tradeFilterProvider),
            new HeistRoomsTotalProperty(game, gameTextProvider, tradeFilterProvider),
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

            new GemLevelProperty(game, gameTextProvider),
            new ItemLevelProperty(game, gameTextProvider),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.SocketCategory?.Title,
                                   new SocketCountProperty(game, gameTextProvider),
                                   new SocketLinkProperty(game, resources)),


            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.RequirementsCategory?.Title,
                                   new RequiresLevelProperty(game, gameTextProvider),
                                   new RequiresStrengthProperty(game, gameTextProvider),
                                   new RequiresDexterityProperty(game, gameTextProvider),
                                   new RequiresIntelligenceProperty(game, gameTextProvider)),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.MiscellaneousCategory?.Title,
                                   new ElderProperty(game, gameTextProvider),
                                   new ShaperProperty(game, gameTextProvider),
                                   new CrusaderProperty(game, gameTextProvider),
                                   new HunterProperty(game, gameTextProvider),
                                   new RedeemerProperty(game, gameTextProvider),
                                   new WarlordProperty(game, gameTextProvider),
                                   new CorruptedProperty(game, gameTextProvider),
                                   new SplitProperty(game, gameTextProvider),
                                   new FracturedProperty(game, tradeFilterProvider),
                                   new DesecratedProperty(game, tradeFilterProvider),
                                   new SanctifiedProperty(game, tradeFilterProvider),
                                   new MirroredProperty(game, gameTextProvider),
                                   new FoulbornProperty(game, tradeFilterProvider),
                                   new ImbuedGemProperty(game, tradeFilterProvider),
                                   new UnidentifiedProperty(game, gameTextProvider)),
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
