using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class CrusaderProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceCrusader.ToRegexLine();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Jewel];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.Influences.Crusader = GetBool(Pattern, parsingItem);
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (!item.Properties.Influences.Crusader) return null;

        var filter = new BooleanPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceCrusader,
            Checked = true,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        searchFilters.GetOrCreateMiscFilters().Filters.CrusaderItem = new SearchFilterOption(filter);
    }
}
