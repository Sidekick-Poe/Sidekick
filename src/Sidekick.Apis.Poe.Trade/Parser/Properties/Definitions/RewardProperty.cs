using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RewardProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IItemDefinitionParser itemDefinitionParser) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionReward.ToRegexStringProperty();

    public override string Label => currentGameLanguage.Language.DescriptionReward;

    public override void Parse(Item item)
    {
        if (item.Definition.ItemClass?.Type != ItemClass.Map) return;
        if (game == GameType.PathOfExile2) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Reward = GetString(Pattern, propertyBlock);
        if (item.Properties.Reward != null) propertyBlock.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Reward == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new RewardFilter(itemDefinitionParser)
        {
            Text = Label,
            Value = item.Properties.Reward!,
            Type = LineContentType.Unique,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RewardProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RewardFilter : StringPropertyFilter
{
    public RewardFilter(IItemDefinitionParser itemDefinitionParser)
    {
        ItemDefinitionParser = itemDefinitionParser;
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    private IItemDefinitionParser ItemDefinitionParser { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        var uniqueItem = ItemDefinitionParser.UniqueItems.FirstOrDefault(x => x.UniqueItem?.Name != null && Value.Contains(x.UniqueItem.Name));
        if (uniqueItem?.UniqueItem?.Name == null) return;

        query.Filters.GetOrCreateMapFilters().Filters.Reward = new SearchFilterOption(uniqueItem.UniqueItem.Name);
    }
}
