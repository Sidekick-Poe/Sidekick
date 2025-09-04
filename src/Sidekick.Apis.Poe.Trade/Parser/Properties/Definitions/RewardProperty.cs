using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RewardProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IApiItemProvider apiItemProvider
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionReward.ToRegexStringCapture();

    public override List<Category> ValidCategories { get; } = [Category.Map];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        if (game == GameType.PathOfExile2) return;

        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.Reward = GetString(Pattern, propertyBlock);
        if (itemProperties.Reward != null) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Reward == null) return null;

        var filter = new StringPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionReward,
            Value = item.Properties.Reward!,
            Type = LineContentType.Unique,
            Checked = true,
        };
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not StringPropertyFilter stringFilter) return;

        var uniqueItem = apiItemProvider.UniqueItems.FirstOrDefault(x => x.Name != null && stringFilter.Value.Contains(x.Name));
        if (uniqueItem?.Name == null) return;

        query.Filters.GetOrCreateMapFilters().Filters.Reward = new SearchFilterOption(uniqueItem.Name);
    }
}
