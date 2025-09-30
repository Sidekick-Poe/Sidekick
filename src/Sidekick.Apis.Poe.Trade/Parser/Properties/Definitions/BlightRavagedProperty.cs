using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightRavagedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.AffixBlightRavaged.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);

    public override List<Category> ValidCategories { get; } = [Category.Map];

    public override void Parse(Item item)
    {
        item.Properties.BlightRavaged = Pattern.IsMatch(item.Text.Blocks[0].Lines[^1].Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (!item.Properties.BlightRavaged) return Task.FromResult<PropertyFilter?>(null);

        var filter = new PropertyFilter(this)
        {
            ShowRow = false,
            Text = gameLanguageProvider.Language.AffixBlightRavaged,
            Checked = true,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.BlightRavavaged = new SearchFilterOption(filter);
    }
}
