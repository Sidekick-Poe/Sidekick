using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightRavagedProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.AffixBlightRavaged.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ItemClass.Map,
    ];

    public override string Label => gameLanguageProvider.Language.AffixBlightRavaged;

    public override void Parse(Item item)
    {
        item.Properties.BlightRavaged = Pattern.IsMatch(item.Text.Blocks[0].Lines[^1].Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.BlightRavaged) return Task.FromResult<TradeFilter?>(null);

        var filter = new BlightRavagedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(BlightRavagedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlightRavagedFilter : TradeFilter
{
    public BlightRavagedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.BlightRavavaged = new SearchFilterOption(this);
    }
}
