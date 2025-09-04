using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Filters;

public interface IFilterProvider : IInitializableService
{
    ApiFilter? TypeCategory { get; }
    ApiFilter? TradePrice { get; }
    ApiFilter? TradeIndexed { get; }
    ApiFilter? Desecrated { get; }
    string? GetPriceOption(string? price);
    string? GetTradeIndexedOption(string? timeFrame);
}
