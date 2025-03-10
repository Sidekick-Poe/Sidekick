using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class EvasionRatingProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionEvasion.ToRegexIntCapture();

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.EvasionRating = GetInt(Pattern, propertyBlock);
        if (itemProperties.EvasionRating > 0) propertyBlock.Parsed = true;
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
