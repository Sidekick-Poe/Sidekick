using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HunterProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceHunter.ToRegexLine();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Influences.Hunter = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Hunter) return Task.FromResult<TradeFilter?>(null);

        var filter = new TradeFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceHunter,
            Checked = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.HunterItem = new SearchFilterOption(filter);
    }
}
