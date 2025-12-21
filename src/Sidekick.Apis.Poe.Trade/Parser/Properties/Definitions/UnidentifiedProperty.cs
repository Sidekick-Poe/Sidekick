using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class UnidentifiedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionUnidentified.ToRegexLine();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Flasks,
        ..ItemClassConstants.Jewels,
        ..ItemClassConstants.Areas,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Unidentified = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Unidentified) return Task.FromResult<TradeFilter?>(null);

        var filter = new UnidentifiedFilter
        {
            Text = gameLanguageProvider.Language.DescriptionUnidentified,
            Checked = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class UnidentifiedFilter : TriStatePropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked is null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Identified = new SearchFilterOption(Checked is true ? "false" : "true");
    }
}
