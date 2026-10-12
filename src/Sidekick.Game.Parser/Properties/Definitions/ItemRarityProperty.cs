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

public class ItemRarityProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyItemRarity.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyItemRarity.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyItemRarity;

    public override void Parse(Item item)
    {
        item.Properties.ItemRarity = GetInt(Pattern, item.Text);
        if (item.Properties.ItemRarity == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.ItemRarity));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ItemRarity <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new ItemRarityFilter
        {
            Text = Label,
            Value = item.Properties.ItemRarity,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ItemRarity)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ItemRarityProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ItemRarityFilter : IntPropertyFilter
{
    public ItemRarityFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.ItemRarity = new StatFilterValue(this);
    }
}
