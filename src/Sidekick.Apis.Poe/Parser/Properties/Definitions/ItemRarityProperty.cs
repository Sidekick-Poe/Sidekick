using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class ItemRarityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Map, Category.Contract];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionItemRarity.ToRegexIntCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.ItemRarity = GetInt(Pattern, propertyBlock);
        if (itemProperties.ItemRarity > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.ItemRarity <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionItemRarity,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.ItemRarity,
            Checked = false,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMapFilters().Filters.ItemRarity = new StatFilterValue(intFilter);
    }
}
