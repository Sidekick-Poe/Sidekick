using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class PropertyParser
(
    IServiceProvider serviceProvider,
    ICurrentGameLanguage currentGameLanguage,
    IItemDefinitionParser itemDefinitionParser,
    ITradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources
) : IPropertyParser
{
    public int Priority => 300;

    private List<PropertyDefinition> Definitions { get; } = new();

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions.Clear();
        Definitions.AddRange([
            new ItemClassProperty(game, resources),
            new RarityProperty(game, currentGameLanguage),

            new SeparatorProperty(),

            new QualityProperty(game, currentGameLanguage),

            new SpiritProperty(game, currentGameLanguage),
            new ArmourProperty(game, currentGameLanguage),
            new EvasionRatingProperty(game, currentGameLanguage),
            new EnergyShieldProperty(game, currentGameLanguage),
            new BlockChanceProperty(game, currentGameLanguage),

            new WeaponDamageProperty(game, currentGameLanguage, serviceProvider, resources),
            new PhysicalDpsProperty(game, resources),
            new ElementalDpsProperty(game, resources),
            new ChaosDpsProperty(game, resources),
            new TotalDpsProperty(game, resources),
            new CriticalHitChanceProperty(game, currentGameLanguage),
            new AttacksPerSecondProperty(game, currentGameLanguage),
            new MemoryStrandsProperty(game, currentGameLanguage),

            new MapTierProperty(game, currentGameLanguage),
            new RewardProperty(game, currentGameLanguage, itemDefinitionParser),
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
            new BlightedProperty(game, currentGameLanguage),
            new BlightRavagedProperty(game, currentGameLanguage),

            new SeparatorProperty(),

            new GemLevelProperty(game, currentGameLanguage),
            new ItemLevelProperty(game, currentGameLanguage),
            new SocketProperty(game, currentGameLanguage, resources),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.RequirementsCategory?.Title,
                                   new RequiresLevelProperty(game, currentGameLanguage),
                                   new RequiresStrengthProperty(game, currentGameLanguage),
                                   new RequiresDexterityProperty(game, currentGameLanguage),
                                   new RequiresIntelligenceProperty(game, currentGameLanguage)),

            new SeparatorProperty(),

            new ExpandableProperty(tradeFilterProvider.MiscellaneousCategory?.Title,
                                   new ElderProperty(game, currentGameLanguage),
                                   new ShaperProperty(game, currentGameLanguage),
                                   new CrusaderProperty(game, currentGameLanguage),
                                   new HunterProperty(game, currentGameLanguage),
                                   new RedeemerProperty(game, currentGameLanguage),
                                   new WarlordProperty(game, currentGameLanguage),
                                   new CorruptedProperty(game, currentGameLanguage),
                                   new FracturedProperty(game, serviceProvider),
                                   new DesecratedProperty(game, serviceProvider),
                                   new SanctifiedProperty(game, serviceProvider),
                                   new MirroredProperty(game, serviceProvider),
                                   new FoulbornProperty(game, serviceProvider),
                                   new ImbuedGemProperty(game, serviceProvider),
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

        return results;
    }
}
