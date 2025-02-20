using Sidekick.Apis.Poe.Filters.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Filters;

public interface IFilterProvider : IInitializableService
{
    List<ApiFilterOption> TypeCategoryOptions { get; }
    List<ApiFilterOption> TradePriceOptions { get; }
    List<ApiFilterOption> TradeIndexedOptions { get; }
    string? GetPriceOption(string? price);
    string? GetTradeIndexedOption(string? timeFrame);
}
