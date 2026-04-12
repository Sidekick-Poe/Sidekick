using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WaystoneDropChanceProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionWaystoneDropChance.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionWaystoneDropChance.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionWaystoneDropChance;

    public override void Parse(Item item)
    {
        if (game != GameType.PathOfExile2) return;

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
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.WaystoneDropChance)) ? LineContentType.Augmented : LineContentType.Simple,
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
