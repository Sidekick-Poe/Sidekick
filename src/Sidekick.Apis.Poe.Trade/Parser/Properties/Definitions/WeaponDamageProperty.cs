using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WeaponDamageProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IStringLocalizer<PoeResources> resources
) : PropertyDefinition
{
    private Regex RangePattern { get; } = new("(\\d+)-(\\d+)", RegexOptions.Compiled);

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Parse(ItemProperties properties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];

        // Parse damage ranges
        foreach (var line in propertyBlock.Lines)
        {
            var isElemental = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionElementalDamage);
            if (isElemental)
            {
                ParseElementalDamage(line, properties);
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

    private static void ParseElementalDamage(ParsingLine line, ItemProperties itemProperties)
    {
        var matches = new Regex(@"(\d+)-(\d+) \((fire|cold|lightning)\)").Matches(line.Text);
        foreach (Match match in matches)
        {
            if (match.Groups.Count < 4)
            {
                continue;
            }

            int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
            int.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);
            var range = new DamageRange(min, max);

            if (match.Groups[3].Value == "fire") itemProperties.FireDamage = range;
            if (match.Groups[3].Value == "cold") itemProperties.ColdDamage = range;
            if (match.Groups[3].Value == "lightning") itemProperties.LightningDamage = range;
        }
    }

    public override List<BooleanPropertyFilter>? GetFilters(Item item, double normalizeValue, FilterType filterType)
    {
        var results = new List<BooleanPropertyFilter>();

        if (item.Properties.TotalDamage > 0)
        {
            var filter = new WeaponDamagePropertyFilter(this)
            {
                Text = resources["Damage"],
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
                Text = resources["PhysicalDps"],
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
                Text = resources["ElementalDps"],
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
                Text = resources["ChaosDps"],
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
                Text = resources["Dps"],
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

        if (filter.Text == resources["Damage"] && filter is DoublePropertyFilter damageFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.Damage = new StatFilterValue(damageFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.Damage = new StatFilterValue(damageFilter); break;
            }
        }

        if (filter.Text == resources["PhysicalDps"] && filter is DoublePropertyFilter physicalDpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.PhysicalDps = new StatFilterValue(physicalDpsFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.PhysicalDps = new StatFilterValue(physicalDpsFilter); break;
            }
        }

        if (filter.Text == resources["ElementalDps"] && filter is DoublePropertyFilter elementalDpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.ElementalDps = new StatFilterValue(elementalDpsFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.ElementalDps = new StatFilterValue(elementalDpsFilter); break;
            }
        }

        if (filter.Text == resources["ChaosDps"] && filter is DoublePropertyFilter chaosDpsFilter)
        {
            // searchFilters.GetOrCreateWeaponFilters().Filters.ChaosDps = new StatFilterValue(chaosDpsFilter);
        }

        if (filter.Text == resources["Dps"] && filter is DoublePropertyFilter dpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.DamagePerSecond = new StatFilterValue(dpsFilter); break;
                case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.DamagePerSecond = new StatFilterValue(dpsFilter); break;
            }
        }
    }
}
