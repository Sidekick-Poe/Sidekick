using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class PropertyParser
(
    IServiceProvider serviceProvider,
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider,
    IFilterProvider filterProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources,
    IInvariantModifierProvider invariantModifierProvider
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

            new WeaponDamageProperty(gameLanguageProvider, game, resources, invariantModifierProvider),
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

            new ExpandablePropertiesDefinition(filterProvider.RequirementsCategory?.Title,
                                               new RequiresLevelProperty(gameLanguageProvider),
                                               new RequiresStrengthProperty(gameLanguageProvider),
                                               new RequiresDexterityProperty(gameLanguageProvider),
                                               new RequiresIntelligenceProperty(gameLanguageProvider)),

            new SeparatorProperty(),

            new ExpandablePropertiesDefinition(filterProvider.MiscellaneousCategory?.Title,
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
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(item.ApiInformation.Category)) continue;

            definition.Parse(item);
        }
    }

    public void ParseAfterModifiers(Item item)
    {
        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(item.ApiInformation.Category)) continue;

            definition.ParseAfterModifiers(item);
        }
    }

    public async Task<List<PropertyFilter>> GetFilters(Item item)
    {
        var results = new List<PropertyFilter>();

        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(item.ApiInformation.Category)) continue;

            var filter = await definition.GetFilter(item);
            if (filter != null) results.Add(filter);

            var filters = definition.GetFilters(item);
            if (filters != null) results.AddRange(filters);
        }

        CleanUpSeparatorFilters(results);

        return results;
    }

    private static void CleanUpSeparatorFilters(List<PropertyFilter> results)
    {
        // Remove leading SeparatorProperty filters
        while (results.Count > 0 && results[0].Definition is SeparatorProperty)
        {
            results.RemoveAt(0);
        }

        // Remove trailing SeparatorProperty filters
        while (results.Count > 0 && results[^1].Definition is SeparatorProperty)
        {
            results.RemoveAt(results.Count - 1);
        }

        // Remove consecutive SeparatorProperty filters
        for (var i = 1; i < results.Count; i++)
        {
            if (results[i].Definition is not SeparatorProperty || results[i - 1].Definition is not SeparatorProperty)
            {
                continue;
            }

            results.RemoveAt(i);
            i--;// Adjust index to recheck current position
        }
    }

    public void PrepareTradeRequest(Query query, Item item, List<PropertyFilter> propertyFilters)
    {
        foreach (var filter in propertyFilters)
        {
            filter.Definition.PrepareTradeRequest(query, item, filter);
        }
    }
}
