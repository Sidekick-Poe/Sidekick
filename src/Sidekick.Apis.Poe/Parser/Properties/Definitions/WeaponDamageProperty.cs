using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class WeaponDamageProperty
(
    IGameLanguageProvider gameLanguageProvider,
    IInvariantModifierProvider invariantModifierProvider
) : PropertyDefinition
{
    private Regex? PhysicalDamagePattern { get; set; }

    private Regex? ElementalDamagePattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Initialize()
    {
        PhysicalDamagePattern = gameLanguageProvider.Language.DescriptionPhysicalDamage.ToRegexStartOfLine();
        ElementalDamagePattern = gameLanguageProvider.Language.DescriptionElementalDamage.ToRegexStartOfLine();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
    }

    public override void ParseAfterModifiers(ItemProperties properties, ParsingItem parsingItem, List<ModifierLine> modifierLines)
    {
        var propertyBlock = parsingItem.Blocks[1];

        // Parse damage ranges
        foreach (var line in propertyBlock.Lines)
        {
            var isElemental = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionElementalDamage);
            if (isElemental)
            {
                ParseElementalDamage(line, properties, modifierLines);
                continue;
            }

            var isPhysical = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionPhysicalDamage);
            var isChaos = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionChaosDamage);
            var isFire = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionFireDamage);
            var isCold = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionColdDamage);
            var isLightning = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionLightningDamage);

            if (!isPhysical && !isChaos && !isFire && !isCold && !isLightning)
            {
                continue;
            }

            var matches = new Regex("(\\d+)-(\\d+)").Matches(line.Text);
            if (matches.Count <= 0 || matches[0].Groups.Count < 3)
            {
                continue;
            }

            double.TryParse(matches[0].Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
            double.TryParse(matches[0].Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);

            var range = new DamageRange(min, max);
            if (isPhysical) properties.PhysicalDamage = range;
            if (isChaos) properties.ChaosDamage = range;
            if (isFire) properties.FireDamage = range;
            if (isCold) properties.ColdDamage = range;
            if (isLightning) properties.LightningDamage = range;
        }
    }

    private void ParseElementalDamage(ParsingLine line, ItemProperties itemProperties, List<ModifierLine> modifierLines)
    {
        var damageMods = invariantModifierProvider.FireWeaponDamageIds.ToList();
        damageMods.AddRange(invariantModifierProvider.ColdWeaponDamageIds);
        damageMods.AddRange(invariantModifierProvider.LightningWeaponDamageIds);

        var itemMods = modifierLines.Where(x => x.Modifiers.Any(y => damageMods.Contains(y.Id ?? string.Empty))).ToList();

        var matches = new Regex("(\\d+)-(\\d+)").Matches(line.Text);
        var matchIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Groups.Count < 3 || itemMods.Count <= matchIndex)
            {
                continue;
            }

            double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
            double.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);
            var range = new DamageRange(min, max);

            var ids = itemMods[matchIndex].Modifiers.Where(x => x.Id != null).Select(x => x.Id!).ToList();
            var isFire = invariantModifierProvider.FireWeaponDamageIds.Any(x => ids.Contains(x));
            if (isFire)
            {
                itemProperties.FireDamage = range;
                matchIndex++;
                continue;
            }

            var isCold = invariantModifierProvider.ColdWeaponDamageIds.Any(x => ids.Contains(x));
            if (isCold)
            {
                itemProperties.ColdDamage = range;
                matchIndex++;
                continue;
            }

            var isLightning = invariantModifierProvider.LightningWeaponDamageIds.Any(x => ids.Contains(x));
            if (isLightning)
            {
                itemProperties.LightningDamage = range;
                matchIndex++;
                continue;
            }

            matchIndex++;
        }
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_Damage, resources.Filters_Damage, item.Properties.TotalDamage, @checked: false, normalizeValue);
        InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_PhysicalDps, resources.Filters_PDps, item.Properties.PhysicalDps, @checked: false, normalizeValue);
        InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_ElementalDps, resources.Filters_EDps, item.Properties.ElementalDps, @checked: false, normalizeValue);
        InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_ChaosDps, resources.Filters_ChaosDps, item.Properties.ChaosDps, @checked: false, normalizeValue);
        InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_Dps, resources.Filters_Dps, item.Properties.TotalDps, @checked: false, normalizeValue);

    }

    internal override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
    }
}
