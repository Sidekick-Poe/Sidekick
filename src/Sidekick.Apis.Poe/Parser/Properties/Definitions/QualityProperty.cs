using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class QualityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Gem, Category.Map];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionQuality.ToRegexIntCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.Quality = GetInt(Pattern, propertyBlock);
        if (itemProperties.Quality > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.GemLevel <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            ShowCheckbox = true,
            Text = gameLanguageProvider.Language.DescriptionQuality,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.Quality,
            Checked = item.Header.Rarity == Rarity.Gem,
        };
        return filter;
    }

    internal override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMiscFilters().Filters.Quality = intFilter.Checked ? new StatFilterValue(intFilter) : null;
    }
}
