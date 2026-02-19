using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Trade;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WeaponDamageProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider,
    IStringLocalizer<PoeResources> resources,
    TradeInvariantStatProvider invariantStatProvider) : PropertyDefinition
{
    private Regex RangePattern { get; } = new(@"([\d,\.]+)-([\d,\.]+)", RegexOptions.Compiled);

    public override string Label => resources["Damage"];

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Weapons.Contains(item.Properties.ItemClass)) return;

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

    public override void ParseAfterStats(Item item)
    {
        if (game == GameType.PathOfExile2) return;

        var damageMods = invariantStatProvider.FireWeaponDamageIds.ToList();
        damageMods.AddRange(invariantStatProvider.ColdWeaponDamageIds);
        damageMods.AddRange(invariantStatProvider.LightningWeaponDamageIds);

        var itemMods = item.Stats.Where(x => x.ApiInformation.Any(y => damageMods.Contains(y.Id ?? string.Empty))).ToList();
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

                var ids = itemMods[matchIndex].ApiInformation.Where(x => x.Id != null).Select(x => x.Id!).ToList();
                var isFire = invariantStatProvider.FireWeaponDamageIds.Any(x => ids.Contains(x));
                var isCold = invariantStatProvider.ColdWeaponDamageIds.Any(x => ids.Contains(x));
                var isLightning = invariantStatProvider.LightningWeaponDamageIds.Any(x => ids.Contains(x));

                if (isFire) item.Properties.FireDamage = range;
                else if (isCold) item.Properties.ColdDamage = range;
                else if (isLightning) item.Properties.LightningDamage = range;

                matchIndex++;
            }
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.TotalDamage <= 0)
        {
            return Task.FromResult<TradeFilter?>(null);
        }

        var filter = new WeaponDamageFilter(game)
        {
            Text = resources["Damage"],
            Value = item.Properties.TotalDamageWithQuality ?? 0,
            OriginalValue = item.Properties.TotalDamage ?? 0,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(WeaponDamageProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };

        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class WeaponDamageFilter : DoublePropertyFilter
{
    public WeaponDamageFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.Damage = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.Damage = new StatFilterValue(this); break;
        }
    }
}
