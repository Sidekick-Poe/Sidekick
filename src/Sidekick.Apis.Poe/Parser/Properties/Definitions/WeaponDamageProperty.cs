using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class WeaponDamageProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IInvariantModifierProvider invariantModifierProvider,
    IStringLocalizer<FilterResources> localizer
) : PropertyDefinition
{
    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Initialize()
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

            int.TryParse(matches[0].Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
            int.TryParse(matches[0].Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);

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

            int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
            int.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);
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

    public override List<BooleanPropertyFilter>? GetFilters(Item item, double normalizeValue)
    {
        var results = new List<BooleanPropertyFilter>();

        if (item.Properties.TotalDamage > 0)
        {
            var filter = new WeaponDamagePropertyFilter(this)
            {
                Text = localizer["Damage"],
                NormalizeEnabled = true,
                NormalizeValue = normalizeValue,
                Value = item.Properties.TotalDamageWithQuality ?? 0,
                OriginalValue = item.Properties.TotalDamage ?? 0,
                Checked = false,
            };
            filter.NormalizeMinValue();
            results.Add(filter);
        }

        if (item.Properties.PhysicalDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = localizer["PhysicalDps"],
                NormalizeEnabled = true,
                NormalizeValue = normalizeValue,
                Value = item.Properties.PhysicalDpsWithQuality ?? 0,
                OriginalValue = item.Properties.PhysicalDps ?? 0,
                Checked = false,
            };
            filter.NormalizeMinValue();
            results.Add(filter);
        }

        if (item.Properties.ElementalDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = localizer["ElementalDps"],
                NormalizeEnabled = true,
                NormalizeValue = normalizeValue,
                Value = item.Properties.ElementalDps ?? 0,
                Checked = false,
            };
            filter.NormalizeMinValue();
            results.Add(filter);
        }

        if (item.Properties.ChaosDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = localizer["ChaosDps"],
                NormalizeEnabled = true,
                NormalizeValue = normalizeValue,
                Value = item.Properties.ChaosDps ?? 0,
                Checked = false,
            };
            filter.NormalizeMinValue();
            results.Add(filter);
        }

        if (item.Properties.TotalDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = localizer["Dps"],
                NormalizeEnabled = true,
                NormalizeValue = normalizeValue,
                Value = item.Properties.TotalDpsWithQuality ?? 0,
                OriginalValue = item.Properties.TotalDps ?? 0,
                Checked = false,
            };
            filter.NormalizeMinValue();
            results.Add(filter);
        }

        return results.Count > 0 ? results : null;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        if (filter.Text == localizer["Damage"] && filter is DoublePropertyFilter damageFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.Damage = new StatFilterValue(damageFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.Damage = new StatFilterValue(damageFilter); break;
            }
        }

        if (filter.Text == localizer["PhysicalDps"] && filter is DoublePropertyFilter physicalDpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.PhysicalDps = new StatFilterValue(physicalDpsFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.PhysicalDps = new StatFilterValue(physicalDpsFilter); break;
            }
        }

        if (filter.Text == localizer["ElementalDps"] && filter is DoublePropertyFilter elementalDpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.ElementalDps = new StatFilterValue(elementalDpsFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.ElementalDps = new StatFilterValue(elementalDpsFilter); break;
            }
        }

        if (filter.Text == localizer["ChaosDps"] && filter is DoublePropertyFilter chaosDpsFilter)
        {
            // searchFilters.GetOrCreateWeaponFilters().Filters.ChaosDps = new StatFilterValue(chaosDpsFilter);
        }

        if (filter.Text == localizer["Dps"] && filter is DoublePropertyFilter dpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.DamagePerSecond = new StatFilterValue(dpsFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.DamagePerSecond = new StatFilterValue(dpsFilter); break;
            }
        }
    }
}
