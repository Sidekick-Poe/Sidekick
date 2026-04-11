using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Data;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Filters.Definitions;

public class CurrencyFilterFactory(ITradeFilterProvider tradeFilterProvider)
{
    private const string SettingKeyPoe1 = "Trade_Filter_Currency_Poe1";
    private const string SettingKeyPoe2 = "Trade_Filter_Currency_Poe2";

    public Task<TradeFilter?> GetFilter(Item item)
    {
        var priceFilters = tradeFilterProvider.GetApiFilter("trade_filters", "price");
        if (priceFilters == null) return Task.FromResult<TradeFilter?>(null);

        var priceKey = item.Game == GameType.PathOfExile1 ? SettingKeyPoe1 : SettingKeyPoe2;

        return Task.FromResult<TradeFilter?>(new CurrencyFilter(priceKey)
        {
            Text = priceFilters.Text ?? string.Empty,
            DefaultValue = null,
            Options = priceFilters.Option.Options
                .Select(x => new OptionFilter.OptionFilterItem(x.Id, x.Text))
                .ToList(),
        });
    }
}

public class CurrencyFilter(string settingKey) : OptionFilter(settingKey)
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!string.IsNullOrEmpty(Value))
        {
            query.Filters.GetOrCreateTradeFilters().Filters.Price = new(Value);
        }
    }
}
