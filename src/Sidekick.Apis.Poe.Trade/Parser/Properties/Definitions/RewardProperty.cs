using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RewardProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game,
    IApiItemProvider apiItemProvider
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionReward.ToRegexStringCapture();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ItemClass.Map,
    ];

    public override void Parse(Item item)
    {
        if (game == GameType.PathOfExile2) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Reward = GetString(Pattern, propertyBlock);
        if (item.Properties.Reward != null) propertyBlock.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Reward == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new RewardFilter(apiItemProvider)
        {
            Text = gameLanguageProvider.Language.DescriptionReward,
            Value = item.Properties.Reward!,
            Type = LineContentType.Unique,
            Checked = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RewardFilter(IApiItemProvider apiItemProvider) : StringPropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        var uniqueItem = apiItemProvider.UniqueItems.FirstOrDefault(x => x.Name != null && Value.Contains(x.Name));
        if (uniqueItem?.Name == null) return;

        query.Filters.GetOrCreateMapFilters().Filters.Reward = new SearchFilterOption(uniqueItem.Name);
    }
}
