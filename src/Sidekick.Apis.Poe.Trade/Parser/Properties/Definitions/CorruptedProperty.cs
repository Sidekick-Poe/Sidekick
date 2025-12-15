using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CorruptedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionCorrupted.ToRegexLine();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Gems,
        ..ItemClassConstants.WithModifiers,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Corrupted = GetBool(Pattern, item.Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        var filter = new TriStatePropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionCorrupted,
            Checked = item.Properties.Corrupted,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStateFilter || triStateFilter.Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Corrupted = new SearchFilterOption(triStateFilter);
    }
}
