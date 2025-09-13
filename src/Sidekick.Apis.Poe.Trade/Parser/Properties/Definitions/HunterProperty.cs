using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HunterProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceHunter.ToRegexLine();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Jewel];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        itemProperties.Influences.Hunter = GetBool(Pattern, parsingItem);
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (!item.Properties.Influences.Hunter) return null;

        var filter = new BooleanPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceHunter,
            Checked = true,
        };
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.HunterItem = new SearchFilterOption(filter);
    }
}
