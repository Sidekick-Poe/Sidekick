using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public class PropertyParser
(
    IServiceProvider serviceProvider,
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider,
    IFilterProvider filterProvider,
    IApiInvariantItemProvider apiInvariantItemProvider,
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
            new ItemQuantityProperty(gameLanguageProvider),
            new ItemRarityProperty(gameLanguageProvider),
            new MonsterPackSizeProperty(gameLanguageProvider),
            new AreaLevelProperty(gameLanguageProvider),
            new BlightedProperty(gameLanguageProvider),
            new BlightRavagedProperty(gameLanguageProvider),

            new SeparatorProperty(),

            new GemLevelProperty(gameLanguageProvider, apiInvariantItemProvider),
            new ItemLevelProperty(gameLanguageProvider, game),
            new SocketProperty(gameLanguageProvider, game, resources),

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
                                                new UnidentifiedProperty(gameLanguageProvider)),
        ]);
    }

    public ItemProperties Parse(ParsingItem parsingItem, ItemHeader header)
    {
        var properties = new ItemProperties();
        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(header.Category)) continue;

            definition.Parse(properties, parsingItem, header);
        }

        return properties;
    }

    public void ParseAfterModifiers(ParsingItem parsingItem, ItemHeader header, ItemProperties properties, List<ModifierLine> modifierLines)
    {
        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(header.Category)) continue;

            definition.ParseAfterModifiers(properties, parsingItem, modifierLines);
        }
    }

    public async Task<List<BooleanPropertyFilter>> GetFilters(Item item)
    {
        var normalizeValue = await settingsService.GetObject<double>(SettingKeys.PriceCheckNormalizeValue);
        var filterType = await settingsService.GetEnum<FilterType>(SettingKeys.PriceCheckDefaultFilterType) ?? FilterType.Minimum;
        var results = new List<BooleanPropertyFilter>();

        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(item.Header.Category)) continue;

            var filter = definition.GetFilter(item, normalizeValue, filterType);
            if (filter != null) results.Add(filter);

            var filters = definition.GetFilters(item, normalizeValue, filterType);
            if (filters != null) results.AddRange(filters);
        }

        CleanUpSeparatorFilters(results);

        return results;
    }

    private static void CleanUpSeparatorFilters(List<BooleanPropertyFilter> results)
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
            i--; // Adjust index to recheck current position
        }
    }

    public void PrepareTradeRequest(Query query, Item item, PropertyFilters propertyFilters)
    {
        foreach (var filter in propertyFilters.Filters)
        {
            filter.Definition.PrepareTradeRequest(query, item, filter);
        }
    }
}
