using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class CurrencyFilterFactory(
    ITradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService)
{
    public async Task<TradeFilter?> GetFilter(Item item)
    {
        var priceFilters = tradeFilterProvider.GetApiFilter("trade_filters", "price");
        var priceKey = item.Game == GameType.PathOfExile1 ? CurrencyFilter.SettingKeyPoe1 : CurrencyFilter.SettingKeyPoe2;
        if (priceFilters != null)
        {
            return new CurrencyFilter()
            {
                Text = priceFilters.Text ?? string.Empty,
                Value = await settingsService.GetString(priceKey),
                DefaultValue = null,
                SettingKey = priceKey,
                Options = priceFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterValue(x.Id, x.Text))
                    .ToList(),
            };
        }

        return null;
    }
}

public class CurrencyFilter : OptionFilter
{
    public const string SettingKeyPoe1 = "Trade_Filter_Currency_Poe1";
    public const string SettingKeyPoe2 = "Trade_Filter_Currency_Poe2";

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!string.IsNullOrEmpty(Value))
        {
            query.Filters.GetOrCreateTradeFilters().Filters.Price = new(Value);
        }
    }
}
