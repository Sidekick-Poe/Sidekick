using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class RewardProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    ItemDefinitionProvider itemDefinitionProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionReward.ToRegexStringProperty();

    public override string Label => currentGameLanguage.Language.DescriptionReward;

    public override void Parse(Item item)
    {
        if (item.ItemClass.Type != ItemClass.Map) return;
        if (game == GameType.PathOfExile2) return;

        item.Properties.Reward = GetString(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2 || item.Properties.Reward == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new RewardFilter(itemDefinitionProvider)
        {
            Text = Label,
            Value = item.Properties.Reward!,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RewardProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RewardFilter : StringPropertyFilter
{
    public RewardFilter(ItemDefinitionProvider itemDefinitionProvider)
    {
        ItemDefinitionProvider = itemDefinitionProvider;
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    private ItemDefinitionProvider ItemDefinitionProvider { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        var uniqueItem = ItemDefinitionProvider.UniqueItems.FirstOrDefault(x => x.Name != null && Value.Contains(x.Name));
        if (uniqueItem?.Name == null) return;

        query.Filters.GetOrCreateMapFilters().Filters.Reward = new SearchFilterOption(uniqueItem.Name);
    }
}
