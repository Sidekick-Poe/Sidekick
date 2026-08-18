using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class ItemQuantityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionItemQuantity.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionItemQuantity.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionItemQuantity;

    public override void Parse(Item item)
    {
        item.Properties.ItemQuantity = GetInt(Pattern, item.Text);
        if (item.Properties.ItemQuantity == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.ItemQuantity));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ItemQuantity <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new ItemQuantityFilter
        {
            Text = Label,
            Value = item.Properties.ItemQuantity,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ItemQuantity)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ItemQuantityProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ItemQuantityFilter : IntPropertyFilter
{
    public ItemQuantityFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.ItemQuantity = new StatFilterValue(this);
    }
}
