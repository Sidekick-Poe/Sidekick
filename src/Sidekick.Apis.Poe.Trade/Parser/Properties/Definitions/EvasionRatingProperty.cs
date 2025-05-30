using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class EvasionRatingProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionEvasion.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionEvasion.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.EvasionRating = GetInt(Pattern, propertyBlock);
        if (itemProperties.EvasionRating == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.EvasionRating));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.EvasionRating <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionEvasion,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.EvasionRatingWithQuality,
            OriginalValue = item.Properties.EvasionRating,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.EvasionRating)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateArmourFilters().Filters.EvasionRating = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.EvasionRating = new StatFilterValue(intFilter); break;
        }
    }
}
