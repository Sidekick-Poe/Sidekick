using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class PlayerStatusFilterFactory(
    ITradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources)
{
    public const string Securable = "securable";
    public const string Available = "available";
    public const string Online = "online";
    public const string Any = "any";
    public const string OnlineLeague = "onlineleague";

    public const string SettingKey = "Trade_Filter_Status";

    public async Task<TradeFilter?> GetFilter(Item item)
    {
        var statusFilters = tradeFilterProvider.GetApiFilter("status_filters", "status");
        var statusValue = await settingsService.GetString(SettingKey);
        if (statusFilters != null)
        {
            var filter = new PlayerStatusFilter(settingsService)
            {
                Text = resources["Player_Status"],
                Value = statusValue ?? Securable,
                DefaultValue = Securable,
                Options = statusFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterValue(x.Id, x.Text))
                    .ToList(),
            };
            return filter;
        }

        return null;
    }
}

public class PlayerStatusFilter(ISettingsService settingsService) : OptionFilter
{
    public override string? DefaultValue => PlayerStatusFilterFactory.Securable;

    public override void PrepareTradeRequest(Query query, Item item)
    {
        query.Status.Option = Value ?? PlayerStatusFilterFactory.Securable;
    }

    public override async Task OnChanged()
    {
        await settingsService.Set(PlayerStatusFilterFactory.SettingKey, Value);
        await base.OnChanged();
    }
}
