using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class HunterProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Jewel, Category.Flask];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.InfluenceHunter.ToRegexLine();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.Influences.Hunter = GetBool(Pattern, parsingItem);
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (!item.Properties.Influences.Hunter) return null;

        var filter = new BooleanPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceHunter,
            Checked = true,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        searchFilters.GetOrCreateMiscFilters().Filters.HunterItem = new SearchFilterOption(filter);
    }
}
