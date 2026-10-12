using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class WaystoneDropChanceProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyWaystoneDropChance.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyWaystoneDropChance.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyWaystoneDropChance;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe2) return;

        item.Properties.WaystoneDropChance = GetInt(Pattern, item.Text);
        if (item.Properties.WaystoneDropChance == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.WaystoneDropChance));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.WaystoneDropChance <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new WaystoneDropChanceFilter
        {
            Text = Label,
            Value = item.Properties.WaystoneDropChance,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.WaystoneDropChance)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(WaystoneDropChanceProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class WaystoneDropChanceFilter : IntPropertyFilter
{
    public WaystoneDropChanceFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.WaystoneDropChance = new StatFilterValue(this);
    }
}
