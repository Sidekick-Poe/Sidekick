using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemQuantityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionItemQuantity.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionItemQuantity.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Map, Category.Contract, Category.Logbook];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.ItemQuantity = GetInt(Pattern, propertyBlock);
        if (itemProperties.ItemQuantity == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.ItemQuantity));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.ItemQuantity <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionItemQuantity,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.ItemQuantity,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.ItemQuantity)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateMapFilters().Filters.ItemQuantity = new StatFilterValue(intFilter);
    }
}
