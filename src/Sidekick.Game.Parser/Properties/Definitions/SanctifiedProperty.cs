using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class SanctifiedProperty(
    GameType game,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    public override string Label => tradeFilterProvider.Sanctified?.Text ?? "Sanctified";

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile1) return Task.FromResult<TradeFilter?>(null);
        if (tradeFilterProvider.Sanctified == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new SanctifiedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(SanctifiedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class SanctifiedFilter : TriStatePropertyFilter
{
    public SanctifiedFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(null);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Sanctified = new SearchFilterOption(this);
    }
}
