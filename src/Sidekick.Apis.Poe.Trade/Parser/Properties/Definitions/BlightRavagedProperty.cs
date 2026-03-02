using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightRavagedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.AffixBlightRavaged.ToRegexAffix(currentGameLanguage.Language.AffixSuperior);

    public override string Label => currentGameLanguage.Language.AffixBlightRavaged;

    public override void Parse(Item item)
    {
        if (item.Properties.ItemClass != ItemClass.Map) return;

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
