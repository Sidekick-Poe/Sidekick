using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Settings;
using Sidekick.Game.Parser.Filters.Definitions;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Apis.Poe.Trade.Filters;

public class TradeFilterParser
(
    TradeFilterProvider tradeFilterProvider,
    ISettingsService settingsService,
    IServiceProvider serviceProvider
)
{
    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (tradeFilterProvider.TradeCategory?.Title == null) return [];

        var result = new List<TradeFilter>();

        var statusFilter = await serviceProvider.GetRequiredService<PlayerStatusFilterFactory>().GetFilter(item);
        if (statusFilter != null)
        {
            result.Add(statusFilter);
            await statusFilter.Initialize(item, settingsService);
        }

        var currencyFilter = await serviceProvider.GetRequiredService<CurrencyFilterFactory>().GetFilter(item);
        if (currencyFilter != null)
        {
            result.Add(currencyFilter);
            await currencyFilter.Initialize(item, settingsService);
        }

        if (result.Count == 0) return [];
        return [new ExpandableFilter(tradeFilterProvider.TradeCategory.Title, false, result.ToArray())];
    }
}
