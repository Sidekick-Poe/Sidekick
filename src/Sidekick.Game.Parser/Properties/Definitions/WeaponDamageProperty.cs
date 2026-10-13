using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
using ItemProperties=Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class WeaponDamageProperty(
    GameType game,
    GameTextProvider gameTextProvider,
    IServiceProvider serviceProvider,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    private readonly StatParser statParser = serviceProvider.GetRequiredService<StatParser>();

    private Regex RangePattern { get; } = new(@"([\d,\.]+)-([\d,\.]+)", RegexOptions.Compiled);

    public override string Label => tradeFilterProvider.Damage?.Text ?? "Damage";

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsWeapon()) return;

        // Parse damage ranges
        foreach (var line in item.Text.Blocks.SelectMany(x => x.Lines))
        {
            var lineText = line.Text.Replace(".", "").Replace(",", "").Trim();

            var isElemental = lineText.StartsWith(gameTextProvider.Texts.ItemPropertyElementalDamage);
            if (isElemental)
            {
                ParseElementalDamage(lineText, item.Properties);
                continue;
            }

            var isPhysical = !string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyPhysicalDamage) && lineText.StartsWith(gameTextProvider.Texts.ItemPropertyPhysicalDamage);
            var isChaos = !string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyChaosDamage) && lineText.StartsWith(gameTextProvider.Texts.ItemPropertyChaosDamage);
            var isFire = !string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyFireDamage) && lineText.StartsWith(gameTextProvider.Texts.ItemPropertyFireDamage);
            var isCold = !string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyColdDamage) && lineText.StartsWith(gameTextProvider.Texts.ItemPropertyColdDamage);
            var isLightning = !string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyLightningDamage) && lineText.StartsWith(gameTextProvider.Texts.ItemPropertyLightningDamage);

            if (!isPhysical && !isChaos && !isFire && !isCold && !isLightning) continue;

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

            line.Parsed = true;
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
        if (game == GameType.Poe2) return;

        var damageMods = statParser.InvariantDetails.FireWeaponDamageIds.ToList();
        damageMods.AddRange(statParser.InvariantDetails.ColdWeaponDamageIds);
        damageMods.AddRange(statParser.InvariantDetails.LightningWeaponDamageIds);

        var itemMods = item.Stats.Where(x =>
        {
            var tradeStats = x.Definitions
                .Where(y => y.TradeIds != null)
                .SelectMany(y => y.TradeIds!)
                .ToList();
            return tradeStats.Any(damageMods.Contains);
        }).ToList();
        if (itemMods.Count == 0) return;

        // Parse elemental damage for Path of Exile 1.
        // In Path of Exile 1, the elemental damage properties have (augmented) as suffix instead of the easier to parse (fire|cold|lightning).
        foreach (var line in item.Text.Blocks.SelectMany(x => x.Lines))
        {
            var isElemental = line.Text.StartsWith(gameTextProvider.Texts.ItemPropertyElementalDamage);
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

                var ids = itemMods[matchIndex].Definitions
                    .Where(x => x.TradeIds != null)
                    .SelectMany(x => x.TradeIds!)
                    .ToList();
                var isFire = statParser.InvariantDetails.FireWeaponDamageIds.Any(ids.Contains);
                var isCold = statParser.InvariantDetails.ColdWeaponDamageIds.Any(ids.Contains);
                var isLightning = statParser.InvariantDetails.LightningWeaponDamageIds.Any(ids.Contains);

                if (isFire) item.Properties.FireDamage = range;
                else if (isCold) item.Properties.ColdDamage = range;
                else if (isLightning) item.Properties.LightningDamage = range;

                matchIndex++;
            }

            line.Parsed = true;
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
            Text = Label,
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
            case GameType.Poe1: query.Filters.GetOrCreateWeaponFilters().Filters.Damage = new StatFilterValue(this); break;
            case GameType.Poe2: query.Filters.GetOrCreateEquipmentFilters().Filters.Damage = new StatFilterValue(this); break;
        }
    }
}
