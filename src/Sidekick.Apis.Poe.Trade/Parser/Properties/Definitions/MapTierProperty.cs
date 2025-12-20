using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MapTierProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMapTier.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ItemClass.Map,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MapTier = GetInt(Pattern, propertyBlock);
        if (item.Properties.MapTier > 0) propertyBlock.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MapTier <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionMapTier,
            NormalizeEnabled = false,
            Value = item.Properties.MapTier,
            Checked = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMapFilters().Filters.MapTier = new StatFilterValue(intFilter);
    }
}
