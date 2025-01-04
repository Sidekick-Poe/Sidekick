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

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.GemLevel <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionQuality,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.Quality,
            Checked = item.Header.Rarity == Rarity.Gem,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMiscFilters().Filters.Quality = new StatFilterValue(intFilter);
    }
}
