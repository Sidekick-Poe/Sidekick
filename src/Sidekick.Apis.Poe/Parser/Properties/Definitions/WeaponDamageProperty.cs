using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class WeaponDamageProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IInvariantModifierProvider invariantModifierProvider,
    IStringLocalizer<PoeResources> localizer
) : PropertyDefinition
{
    private Regex RangePattern { get; } = new("(\\d+)-(\\d+)", RegexOptions.Compiled);

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

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

            if (isPhysical && line.Text.EndsWith(")")) properties.AugmentedProperties.Add(nameof(ItemProperties.PhysicalDamage));
            if (isChaos && line.Text.EndsWith(")")) properties.AugmentedProperties.Add(nameof(ItemProperties.ChaosDamage));
            if (isFire && line.Text.EndsWith(")")) properties.AugmentedProperties.Add(nameof(ItemProperties.FireDamage));
            if (isCold && line.Text.EndsWith(")")) properties.AugmentedProperties.Add(nameof(ItemProperties.ColdDamage));
            if (isLightning && line.Text.EndsWith(")")) properties.AugmentedProperties.Add(nameof(ItemProperties.LightningDamage));

            var matches = RangePattern.Matches(line.Text);
            if (matches.Count <= 0 || matches[0].Groups.Count < 3) continue;

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

        var itemMods = modifierLines.Where(x => x.Modifiers.Any(y => damageMods.Contains(y.ApiId ?? string.Empty))).ToList();

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

            var ids = itemMods[matchIndex].Modifiers.Where(x => x.ApiId != null).Select(x => x.ApiId!).ToList();
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

    public override List<BooleanPropertyFilter>? GetFilters(Item item, double normalizeValue, FilterType filterType)
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
            filter.ChangeFilterType(filterType);
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
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            filter.ChangeFilterType(filterType);
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
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.FireDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ColdDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.LightningDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            filter.ChangeFilterType(filterType);
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
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            filter.ChangeFilterType(filterType);
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
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.FireDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ColdDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.LightningDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            filter.ChangeFilterType(filterType);
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
