using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class PlayerStatusFilter(
    ITradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources)
{
    public const string Securable = "securable";
    public const string Available = "available";
    public const string Online = "online";
    public const string Any = "any";
    public const string OnlineLeague = "onlineleague";

    private const string SettingKey = "Trade_Filter_Status";

    public async Task<TradeFilter?> GetFilter(Item item)
    {
        var statusFilters = tradeFilterProvider.GetApiFilter("status_filters", "status");
        var statusValue = await settingsService.GetString(SettingKey);
        if (statusFilters != null)
        {
            var filter = new OptionFilter()
            {
                Text = resources["Player_Status"],
                Value = statusValue ?? Securable,
                DefaultValue = Securable,
                SettingKey = SettingKey,
                Options = statusFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterValue(x.Id, x.Text))
                    .ToList(),
            };
            filter.PrepareTradeRequest = (query, _) => query.Status.Option = filter.Value ?? Securable;
            return filter;
        }

        return null;
    }
}
