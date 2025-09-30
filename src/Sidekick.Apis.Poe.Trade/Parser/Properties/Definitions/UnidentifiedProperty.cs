using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class UnidentifiedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionUnidentified.ToRegexLine();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Map, Category.Contract, Category.Accessory, Category.Jewel];

    public override void Parse(Item item)
    {
        item.Properties.Unidentified = GetBool(Pattern, item.Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (!item.Properties.Unidentified)
        {
            return Task.FromResult<PropertyFilter?>(null);
        }

        return Task.FromResult<PropertyFilter?>(new(this)
        {
            Text = gameLanguageProvider.Language.DescriptionUnidentified,
            Checked = true,
        });
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
