using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class UnidentifiedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Map, Category.Contract, Category.Accessory, Category.Jewel];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionUnidentified.ToRegexLine();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.Unidentified = GetBool(Pattern, parsingItem);
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue) => new(this)
    {
        Text = gameLanguageProvider.Language.DescriptionUnidentified,
        Checked = !item.Properties.Unidentified,
    };

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        searchFilters.GetOrCreateMiscFilters().Filters.Identified = new SearchFilterOption(item.Properties.Unidentified ? "false" : "true");
    }
}
