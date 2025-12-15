using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

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

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Unidentified) return Task.FromResult<PropertyFilter?>(null);

        var filter = new TriStatePropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionUnidentified,
            Checked = true,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStatePropertyFilter || triStatePropertyFilter.Checked is null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Identified = new SearchFilterOption(triStatePropertyFilter.Checked is true ? "false" : "true");
    }
}
