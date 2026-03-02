using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MapTierProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMapTier.ToRegexIntCapture();

    public override string Label => currentGameLanguage.Language.DescriptionMapTier;

    public override void Parse(Item item)
    {
        if (item.Properties.ItemClass != ItemClass.Map) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MapTier = GetInt(Pattern, propertyBlock);
        if (item.Properties.MapTier > 0) propertyBlock.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MapTier <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MapTierFilter
        {
            Text = Label,
            Value = item.Properties.MapTier,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MapTierProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MapTierFilter : IntPropertyFilter
{
    public MapTierFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.MapTier = new StatFilterValue(this);
    }
}
