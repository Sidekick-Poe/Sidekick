using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.AffixBlighted.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);

    public override List<ItemClass> ValidItemClasses { get; } = [
        ItemClass.Map,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Blighted = Pattern.IsMatch(item.Text.Blocks[0].Lines[^1].Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Blighted) return Task.FromResult<TradeFilter?>(null);

        var filter = new BlightedFilter
        {
            ShowRow = false,
            Text = gameLanguageProvider.Language.AffixBlighted,
            Checked = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlightedFilter : TradeFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.Blighted = new SearchFilterOption(this);
    }
}
