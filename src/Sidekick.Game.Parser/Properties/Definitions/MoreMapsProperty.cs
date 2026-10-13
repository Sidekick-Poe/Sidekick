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

public class MoreMapsProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyMoreMaps.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyMoreMaps.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyMoreMaps;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe1) return;

        item.Properties.MoreMaps = GetInt(Pattern, item.Text);
        if (item.Properties.MoreMaps == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MoreMaps));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.Poe1 || item.Properties.MoreMaps <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MoreMapsFilter
        {
            Text = Label,
            Value = item.Properties.MoreMaps,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MoreMaps)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MoreMapsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MoreMapsFilter : IntPropertyFilter
{
    public MoreMapsFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = "pseudo.pseudo_map_more_map_drops",
            Value = new StatFilterValue(this),
        });
    }
}
