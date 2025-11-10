using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.AffixBlighted.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);

    public override List<Category> ValidCategories { get; } = [Category.Map];

    public override void Parse(Item item)
    {
        item.Properties.Blighted = Pattern.IsMatch(item.Text.Blocks[0].Lines[^1].Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Blighted) return Task.FromResult<PropertyFilter?>(null);

        var filter = new PropertyFilter(this)
        {
            ShowRow = false,
            Text = gameLanguageProvider.Language.AffixBlighted,
            Checked = true,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.Blighted = new SearchFilterOption(filter);
    }
}
