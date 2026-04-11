using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.AffixBlighted.ToRegexAffix(currentGameLanguage.Language.AffixSuperior);

    public override string Label => currentGameLanguage.Language.AffixBlighted;

    public override void Parse(Item item)
    {
        if (item.ItemClass.Type != ItemClass.Map) return;

        item.Properties.Blighted = Pattern.IsMatch(item.Text.Blocks[0].Lines[^1].Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Blighted) return Task.FromResult<TradeFilter?>(null);

        var filter = new BlightedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(BlightedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlightedFilter : HiddenFilter
{
    public BlightedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.Blighted = new SearchFilterOption(this);
    }
}
