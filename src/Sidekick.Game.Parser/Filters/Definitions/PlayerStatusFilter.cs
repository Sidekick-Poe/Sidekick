using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Localization;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Filters.Definitions;

public class PlayerStatusFilterFactory(
    TradeFilterProvider tradeFilterProvider,
    IStringLocalizer<PoeResources> resources)
{
    public const string Securable = "securable";
    public const string Available = "available";
    public const string Online = "online";
    public const string Any = "any";
    public const string OnlineLeague = "onlineleague";

    public const string SettingKey = "Trade_Filter_Status";

    public Task<TradeFilter?> GetFilter(Item item)
    {
        var statusFilters = tradeFilterProvider.GetApiFilter("status_filters", "status");
        if (statusFilters == null) return Task.FromResult<TradeFilter?>(null);

            return Task.FromResult<TradeFilter?>(new PlayerStatusFilter()
            {
                Text = resources["Player_Status"],
                Options = statusFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterItem(x.Id, x.Text))
                    .ToList(),
            });
    }
}

public class PlayerStatusFilter() : OptionFilter(PlayerStatusFilterFactory.SettingKey)
{
    public override string DefaultValue => PlayerStatusFilterFactory.Securable;

    public override void PrepareTradeRequest(Query query, Item item)
    {
        query.Status.Option = Value ?? PlayerStatusFilterFactory.Securable;
    }
}
