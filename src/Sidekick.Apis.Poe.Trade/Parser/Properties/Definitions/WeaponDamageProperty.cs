using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WeaponDamageProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IStringLocalizer<PoeResources> resources,
    IInvariantModifierProvider invariantModifierProvider
) : PropertyDefinition
{
    private Regex RangePattern { get; } = new(@"([\d,\.]+)-([\d,\.]+)", RegexOptions.Compiled);

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];

        // Parse damage ranges
        foreach (var line in propertyBlock.Lines)
        {
            var lineText = line.Text.Replace(".", "").Replace(",", "").Trim();

            var isElemental = lineText.StartsWith(gameLanguageProvider.Language.DescriptionElementalDamage);
            if (isElemental)
            {
                ParseElementalDamage(lineText, item.Properties);
                continue;
            }

            var isPhysical = lineText.StartsWith(gameLanguageProvider.Language.DescriptionPhysicalDamage);
            var isChaos = lineText.StartsWith(gameLanguageProvider.Language.DescriptionChaosDamage);
            var isFire = lineText.StartsWith(gameLanguageProvider.Language.DescriptionFireDamage);
            var isCold = lineText.StartsWith(gameLanguageProvider.Language.DescriptionColdDamage);
            var isLightning = lineText.StartsWith(gameLanguageProvider.Language.DescriptionLightningDamage);

            if (!isPhysical && !isChaos && !isFire && !isCold && !isLightning)
            {
                continue;
            }

            if (isPhysical && lineText.EndsWith(")")) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.PhysicalDamage));
            if (isChaos && lineText.EndsWith(")")) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.ChaosDamage));
            if (isFire && lineText.EndsWith(")")) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.FireDamage));
            if (isCold && lineText.EndsWith(")")) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.ColdDamage));
            if (isLightning && lineText.EndsWith(")")) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.LightningDamage));

            var matches = RangePattern.Matches(lineText);
            if (matches.Count <= 0 || matches[0].Groups.Count < 3) continue;

            int.TryParse(matches[0].Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
            int.TryParse(matches[0].Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);

            var range = new DamageRange(min, max);
            if (isPhysical) item.Properties.PhysicalDamage = range;
            if (isChaos) item.Properties.ChaosDamage = range;
            if (isFire) item.Properties.FireDamage = range;
            if (isCold) item.Properties.ColdDamage = range;
            if (isLightning) item.Properties.LightningDamage = range;
        }
    }

    private static void ParseElementalDamage(string line, ItemProperties itemProperties)
    {
        var matches = new Regex(@"([\d,\.]+)-([\d,\.]+) \((fire|cold|lightning)\)").Matches(line);
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

    public override void ParseAfterModifiers(Item item)
    {
        if (game == GameType.PathOfExile2) return;

        var damageMods = invariantModifierProvider.FireWeaponDamageIds.ToList();
        damageMods.AddRange(invariantModifierProvider.ColdWeaponDamageIds);
        damageMods.AddRange(invariantModifierProvider.LightningWeaponDamageIds);

        var itemMods = item.Modifiers.Where(x => x.ApiInformation.Any(y => damageMods.Contains(y.ApiId ?? string.Empty))).ToList();
        if (itemMods.Count == 0) return;

        // Parse elemental damage for Path of Exile 1.
        // In Path of Exile 1, the elemental damage properties have (augmented) as suffix instead of the easier to parse (fire|cold|lightning).
        foreach (var line in item.Text.Blocks[1].Lines)
        {
            var isElemental = line.Text.StartsWith(gameLanguageProvider.Language.DescriptionElementalDamage);
            if (!isElemental) continue;

            var lineText = line.Text.Replace(".", "").Replace(",", "").Trim();
            var matches = new Regex(@"([\d,\.]+)-([\d,\.]+) \(augmented\)").Matches(lineText);
            var matchIndex = 0;
            foreach (Match match in matches)
            {
                if (match.Groups.Count < 3 || itemMods.Count <= matchIndex) continue;

                int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var min);
                int.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var max);
                var range = new DamageRange(min, max);

                var ids = itemMods[matchIndex].ApiInformation.Where(x => x.ApiId != null).Select(x => x.ApiId!).ToList();
                var isFire = invariantModifierProvider.FireWeaponDamageIds.Any(x => ids.Contains(x));
                var isCold = invariantModifierProvider.ColdWeaponDamageIds.Any(x => ids.Contains(x));
                var isLightning = invariantModifierProvider.LightningWeaponDamageIds.Any(x => ids.Contains(x));

                if (isFire) item.Properties.FireDamage = range;
                else if (isCold) item.Properties.ColdDamage = range;
                else if (isLightning) item.Properties.LightningDamage = range;

                matchIndex++;
            }
        }
    }

    public override List<PropertyFilter>? GetFilters(Item item)
    {
        var results = new List<PropertyFilter>();

        if (item.Properties.TotalDamage > 0)
        {
            var filter = new WeaponDamagePropertyFilter(this)
            {
                Text = resources["Damage"],
                NormalizeEnabled = true,
                Value = item.Properties.TotalDamageWithQuality ?? 0,
                OriginalValue = item.Properties.TotalDamage ?? 0,
                Checked = false,
            };
            results.Add(filter);
        }

        if (item.Properties.PhysicalDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = resources["PhysicalDps"],
                NormalizeEnabled = true,
                Value = item.Properties.PhysicalDpsWithQuality ?? 0,
                OriginalValue = item.Properties.PhysicalDps ?? 0,
                Checked = false,
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            results.Add(filter);
        }

        if (item.Properties.ElementalDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = resources["ElementalDps"],
                NormalizeEnabled = true,
                Value = item.Properties.ElementalDps ?? 0,
                Checked = false,
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.FireDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ColdDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.LightningDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            results.Add(filter);
        }

        if (item.Properties.ChaosDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = resources["ChaosDps"],
                NormalizeEnabled = true,
                Value = item.Properties.ChaosDps ?? 0,
                Checked = false,
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            results.Add(filter);
        }

        if (item.Properties.TotalDps > 0)
        {
            var filter = new DoublePropertyFilter(this)
            {
                Text = resources["Dps"],
                NormalizeEnabled = true,
                Value = item.Properties.TotalDpsWithQuality ?? 0,
                OriginalValue = item.Properties.TotalDps ?? 0,
                Checked = false,
                Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.PhysicalDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.FireDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ColdDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.LightningDamage)) || item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ChaosDamage)) ? LineContentType.Augmented : LineContentType.Simple,
            };
            results.Add(filter);
        }

        return results.Count > 0 ? results : null;
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked) return;

        if (filter.Text == resources["Damage"] && filter is DoublePropertyFilter damageFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.Damage = new StatFilterValue(damageFilter); break;
                case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.Damage = new StatFilterValue(damageFilter); break;
            }
        }

        if (filter.Text == resources["PhysicalDps"] && filter is DoublePropertyFilter physicalDpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.PhysicalDps = new StatFilterValue(physicalDpsFilter); break;
                case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.PhysicalDps = new StatFilterValue(physicalDpsFilter); break;
            }
        }

        if (filter.Text == resources["ElementalDps"] && filter is DoublePropertyFilter elementalDpsFilter)
        {
            switch (game)
            {
                case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.ElementalDps = new StatFilterValue(elementalDpsFilter); break;
                case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.ElementalDps = new StatFilterValue(elementalDpsFilter); break;
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
                case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.DamagePerSecond = new StatFilterValue(dpsFilter); break;
                case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.DamagePerSecond = new StatFilterValue(dpsFilter); break;
            }
        }
    }
}
