using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class GemLevelProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IApiInvariantItemProvider apiInvariantItemProvider
) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Gem];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.GemLevel = GetInt(Pattern, propertyBlock);
        if (itemProperties.GemLevel > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.GemLevel <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            ShowCheckbox = true,
            Text = gameLanguageProvider.Language.DescriptionLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.GemLevel,
            Checked = true,
        };
        return filter;
    }

    internal override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue(intFilter); break;

            case GameType.PathOfExile2 when apiInvariantItemProvider.UncutGemIds.Contains(item.Header.ApiItemId ?? string.Empty): searchFilters.TypeFilters.Filters.ItemLevel = new StatFilterValue(intFilter); break;

            case GameType.PathOfExile2: searchFilters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue(intFilter); break;
        }
    }
}
