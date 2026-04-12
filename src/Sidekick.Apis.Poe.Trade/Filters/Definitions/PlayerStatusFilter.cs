using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Filters.Definitions;

public class PlayerStatusFilterFactory(
    ITradeFilterProvider tradeFilterProvider,
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
