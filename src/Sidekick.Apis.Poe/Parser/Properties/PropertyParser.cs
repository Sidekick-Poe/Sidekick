using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties;

public class PropertyParser
(
    IGameLanguageProvider gameLanguageProvider,
    IInvariantModifierProvider invariantModifierProvider,
    IApiInvariantItemProvider apiInvariantItemProvider,
    ISettingsService settingsService,
    IStringLocalizer<FilterResources> filterLocalizer
) : IPropertyParser
{
    public int Priority => 200;

    private List<PropertyDefinition> Definitions { get; set; } = new();

    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        Definitions.Clear();
        Definitions.AddRange([
            new QualityProperty(gameLanguageProvider),

            new ArmourProperty(gameLanguageProvider, game),
            new EvasionRatingProperty(gameLanguageProvider, game),
            new EnergyShieldProperty(gameLanguageProvider, game),
            new BlockChanceProperty(gameLanguageProvider, game),

            new WeaponDamageProperty(gameLanguageProvider, game, invariantModifierProvider, filterLocalizer),
            new AttacksPerSecondProperty(gameLanguageProvider, game),
            new CriticalHitChanceProperty(gameLanguageProvider, game),

            new ItemQuantityProperty(gameLanguageProvider),
            new ItemRarityProperty(gameLanguageProvider),
            new MonsterPackSizeProperty(gameLanguageProvider),
            new BlightedProperty(gameLanguageProvider),
            new BlightRavagedProperty(gameLanguageProvider),
            new MapTierProperty(gameLanguageProvider),
            new AreaLevelProperty(gameLanguageProvider),

            new SeparatorProperty(),

            new GemLevelProperty(gameLanguageProvider, game, apiInvariantItemProvider),
            new ItemLevelProperty(gameLanguageProvider, game),
            new CorruptedProperty(gameLanguageProvider),
            new UnidentifiedProperty(gameLanguageProvider),

            new ElderProperty(gameLanguageProvider),
            new ShaperProperty(gameLanguageProvider),
            new CrusaderProperty(gameLanguageProvider),
            new HunterProperty(gameLanguageProvider),
            new RedeemerProperty(gameLanguageProvider),
            new WarlordProperty(gameLanguageProvider),
        ]);

        foreach (var definition in Definitions)
        {
            definition.Initialize();
        }
    }

    public ItemProperties Parse(ParsingItem parsingItem)
    {
        var properties = new ItemProperties();
        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(parsingItem.Header?.Category ?? Category.Unknown)) continue;

            definition.Parse(properties, parsingItem);
        }

        return properties;
    }

    public void ParseAfterModifiers(ParsingItem parsingItem, ItemProperties properties, List<ModifierLine> modifierLines)
    {
        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(parsingItem.Header?.Category ?? Category.Unknown)) continue;

            definition.ParseAfterModifiers(properties, parsingItem, modifierLines);
        }
    }

    public async Task<List<BooleanPropertyFilter>> GetFilters(Item item)
    {
        var normalizeValue = await settingsService.GetObject<double>(SettingKeys.PriceCheckNormalizeValue);
        var results = new List<BooleanPropertyFilter>();

        foreach (var definition in Definitions)
        {
            if (definition.ValidCategories.Count > 0 && !definition.ValidCategories.Contains(item.Header.Category)) continue;

            var filter = definition.GetFilter(item, normalizeValue);
            if (filter != null) results.Add(filter);

            var filters = definition.GetFilters(item, normalizeValue);
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

    public void PrepareTradeRequest(SearchFilters searchFilters, Item item, PropertyFilters propertyFilters)
    {
        foreach (var filter in propertyFilters.Filters)
        {
            filter.Definition.PrepareTradeRequest(searchFilters, item, filter);
        }
    }
}
