using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CrusaderProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.InfluenceCrusader.ToRegexLine();

    public override string Label => currentGameLanguage.Language.InfluenceCrusader;

    public override void Parse(Item item)
    {
        item.Properties.Influences.Crusader = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Crusader) return Task.FromResult<TradeFilter?>(null);

        var filter = new CrusaderFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(CrusaderProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class CrusaderFilter : TradeFilter
{
    public CrusaderFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.CrusaderItem = new SearchFilterOption(this);
    }
}
