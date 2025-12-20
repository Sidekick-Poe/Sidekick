using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class CurrencyFilter(
    ITradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService)
{
    public const string SettingKeyPoe1 = "Trade_Filter_Currency_Poe1";
    public const string SettingKeyPoe2 = "Trade_Filter_Currency_Poe2";

    public async Task<TradeFilter?> GetFilter(Item item)
    {
        var priceFilters = tradeFilterProvider.GetApiFilter("trade_filters", "price");
        var priceKey = item.Game == GameType.PathOfExile1 ? SettingKeyPoe1 : SettingKeyPoe2;
        var priceValue = await settingsService.GetString(priceKey);
        if (priceFilters != null)
        {
            var filter = new OptionFilter()
            {
                Text = priceFilters.Text ?? string.Empty,
                Value = priceValue,
                DefaultValue = null,
                SettingKey = priceKey,
                Options = priceFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterValue(x.Id, x.Text))
                    .ToList(),
            };
            filter.PrepareTradeRequest = (query, _) =>
            {
                var option = tradeFilterProvider.GetPriceOption(filter.Value);
                if (!string.IsNullOrEmpty(option))
                {
                    query.Filters.GetOrCreateTradeFilters().Filters.Price = new(option);
                }
            };
           return filter;
        }

        return null;
    }
}
