using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RewardProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionReward.ToRegexStringCapture();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ItemClass.Map,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionReward;

    public override void Parse(Item item)
    {
        if (game == GameType.PathOfExile2) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Reward = GetString(Pattern, propertyBlock);
        if (item.Properties.Reward != null) propertyBlock.Parsed = true;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Reward == null) return null;

        var filter = new RewardFilter(apiItemProvider)
        {
            Text = Label,
            Value = item.Properties.Reward!,
            Type = LineContentType.Unique,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RewardProperty)}_{game.GetValueAttribute()}",
        };
        return filter;
    }
}

public class RewardFilter : StringPropertyFilter
{
    public RewardFilter(IApiItemProvider apiItemProvider)
    {
        ApiItemProvider = apiItemProvider;
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    private IApiItemProvider ApiItemProvider { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        var uniqueItem = ApiItemProvider.UniqueItems.FirstOrDefault(x => x.Name != null && Value.Contains(x.Name));
        if (uniqueItem?.Name == null) return;

        query.Filters.GetOrCreateMapFilters().Filters.Reward = new SearchFilterOption(uniqueItem.Name);
    }
}
