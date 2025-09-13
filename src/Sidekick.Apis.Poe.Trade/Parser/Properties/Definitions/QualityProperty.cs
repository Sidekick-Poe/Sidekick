using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class QualityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionQuality.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionQuality.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Gem, Category.Map];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.Quality = GetInt(Pattern, propertyBlock);
        if (itemProperties.Quality == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.Quality));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.Quality <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionQuality,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.Quality,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = item.Header.Rarity == Rarity.Gem,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Quality)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMiscFilters().Filters.Quality = new StatFilterValue(intFilter);
    }
}
