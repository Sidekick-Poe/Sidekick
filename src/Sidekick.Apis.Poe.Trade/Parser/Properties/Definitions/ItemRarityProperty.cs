using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemRarityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionItemRarity.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionItemRarity.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionItemRarity;

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
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ItemRarity)) ? LineContentType.Augmented : LineContentType.Simple,
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
