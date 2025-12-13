using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ShaperProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceShaper.ToRegexLine();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Weapons,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Influences.Shaper = GetBool(Pattern, item.Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Shaper) return Task.FromResult<PropertyFilter?>(null);

        var filter = new PropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceShaper,
            Checked = true,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.ShaperItem = new SearchFilterOption(filter);
    }
}
