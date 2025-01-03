using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties.Definitions;
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
    ISettingsService settingsService
) : IPropertyParser
{
    public int Priority => 200;

    private List<PropertyDefinition> Definitions { get; set; } = new();

    private Regex? MapTier { get; set; }

    private Regex? ItemQuantity { get; set; }

    private Regex? ItemRarity { get; set; }

    private Regex? MonsterPackSize { get; set; }

    private Regex? AttacksPerSecond { get; set; }

    private Regex? CriticalStrikeChance { get; set; }

    private Regex? Blighted { get; set; }

    private Regex? BlightRavaged { get; set; }

    private Regex? AreaLevel { get; set; }

    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        Definitions.Clear();
        Definitions.AddRange([
            new ArmourProperty(gameLanguageProvider),
            new BlockChanceProperty(gameLanguageProvider, game),
            new EvasionRatingProperty(gameLanguageProvider),
            new EnergyShieldProperty(gameLanguageProvider),
            new GemLevelProperty(gameLanguageProvider, game, apiInvariantItemProvider),
            new CorruptedProperty(gameLanguageProvider),
            new ItemLevelProperty(gameLanguageProvider, game),
            new QualityProperty(gameLanguageProvider),
            new UnidentifiedProperty(gameLanguageProvider),
            new WeaponDamageProperty(gameLanguageProvider, invariantModifierProvider),
        ]);

        foreach (var definition in Definitions)
        {
            definition.Initialize();
        }

        AttacksPerSecond = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexDecimalCapture();
        CriticalStrikeChance = gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexDecimalCapture();

        MapTier = gameLanguageProvider.Language.DescriptionMapTier.ToRegexIntCapture();
        AreaLevel = gameLanguageProvider.Language.DescriptionAreaLevel.ToRegexIntCapture();
        ItemQuantity = gameLanguageProvider.Language.DescriptionItemQuantity.ToRegexIntCapture();
        ItemRarity = gameLanguageProvider.Language.DescriptionItemRarity.ToRegexIntCapture();
        MonsterPackSize = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIntCapture();
        Blighted = gameLanguageProvider.Language.AffixBlighted.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);
        BlightRavaged = gameLanguageProvider.Language.AffixBlightRavaged.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);
    }

    public ItemProperties Parse(ParsingItem parsingItem)
    {
        var properties = new ItemProperties();
        foreach (var definition in Definitions)
        {
            if (!definition.ValidCategories.Contains(parsingItem.Header?.Category ?? Category.Unknown)) continue;

            definition.Parse(properties, parsingItem);
        }

        return properties;
        return parsingItem.Header?.Category switch
        {
            Category.Map or Category.Contract => ParseMapProperties(parsingItem),
            Category.Weapon => ParseWeaponProperties(parsingItem, modifierLines),
            Category.Sanctum => ParseSanctumProperties(parsingItem),
            Category.Logbook => ParseLogbookProperties(parsingItem),
            _ => new ItemProperties(),
        };
    }


    public void ParseAfterModifiers(ParsingItem parsingItem, ItemProperties properties, List<ModifierLine> modifierLines)
    {
        foreach (var definition in Definitions)
        {
            if (!definition.ValidCategories.Contains(parsingItem.Header?.Category ?? Category.Unknown)) continue;

            definition.ParseAfterModifiers(properties, parsingItem, modifierLines);
        }
    }

    private ItemProperties ParseMapProperties(ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];

        return new ItemProperties()
        {
            Blighted = Blighted?.IsMatch(parsingItem.Blocks[0].Lines[^1].Text) ?? false,
            BlightRavaged = BlightRavaged?.IsMatch(parsingItem.Blocks[0].Lines[^1].Text) ?? false,
            ItemQuantity = GetInt(ItemQuantity, propertyBlock),
            ItemRarity = GetInt(ItemRarity, propertyBlock),
            MonsterPackSize = GetInt(MonsterPackSize, propertyBlock),
            MapTier = GetInt(MapTier, propertyBlock),
        };
    }

    private ItemProperties ParseSanctumProperties(ParsingItem parsingItem)
    {
        return new ItemProperties
        {
            AreaLevel = GetInt(AreaLevel, parsingItem),
        };
    }

    private ItemProperties ParseLogbookProperties(ParsingItem parsingItem)
    {
        return new ItemProperties
        {
            AreaLevel = GetInt(AreaLevel, parsingItem),
        };
    }
}
