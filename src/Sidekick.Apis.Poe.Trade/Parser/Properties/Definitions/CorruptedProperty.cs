using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CorruptedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionCorrupted.ToRegexLine();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Map, Category.Contract, Category.Jewel, Category.Flask, Category.Gem];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.Corrupted = GetBool(Pattern, parsingItem);
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        var filter = new TriStatePropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionCorrupted,
            Checked = null,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStateFilter || triStateFilter.Checked == null)
        {
            return;
        }

        searchFilters.GetOrCreateMiscFilters().Filters.Corrupted = new SearchFilterOption(triStateFilter);
    }
}
