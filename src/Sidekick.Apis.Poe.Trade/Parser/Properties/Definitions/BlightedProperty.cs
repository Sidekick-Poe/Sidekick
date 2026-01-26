using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlightedProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.AffixBlighted.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ItemClass.Map,
    ];

    public override string Label => gameLanguageProvider.Language.AffixBlighted;

    public override void Parse(Item item)
    {
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
