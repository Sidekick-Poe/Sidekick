using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class UnidentifiedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionUnidentified.ToRegexLine();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Map, Category.Contract, Category.Accessory, Category.Jewel];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.Unidentified = GetBool(Pattern, parsingItem);
    }

    public override TriStatePropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (!item.Properties.Unidentified)
        {
            return null;
        }

        return new(this)
        {
            Text = gameLanguageProvider.Language.DescriptionUnidentified,
            Checked = true,
        };
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStatePropertyFilter || triStatePropertyFilter.Checked is null)
        {
            return;
        }

        searchFilters.GetOrCreateMiscFilters().Filters.Identified = new SearchFilterOption(triStatePropertyFilter.Checked is true ? "false" : "true");
    }
}
