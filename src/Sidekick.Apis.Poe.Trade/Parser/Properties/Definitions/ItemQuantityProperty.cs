using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemQuantityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionItemQuantity.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionItemQuantity.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionItemQuantity;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Areas.Contains(item.Properties.ItemClass)) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.ItemQuantity = GetInt(Pattern, propertyBlock);
        if (item.Properties.ItemQuantity == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.ItemQuantity));
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
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ItemQuantity)) ? LineContentType.Augmented : LineContentType.Simple,
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
