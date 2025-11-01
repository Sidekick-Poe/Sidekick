using Sidekick.Apis.Poe.Trade.Filters.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Filters;

public interface IFilterProvider : IInitializableService
{
    ApiFilter? TypeCategory { get; }
    ApiFilter? TradePrice { get; }
    ApiFilter? TradeIndexed { get; }
    ApiFilter? Desecrated { get; }
    ApiFilter? Veiled { get; }
    ApiFilter? Fractured { get; }
    ApiFilter? Mirrored { get; }
    ApiFilter? Foulborn { get; }
    ApiFilter? Sanctified { get; }
    ApiFilterCategory? RequirementsCategory { get; }
    ApiFilterCategory? MiscellaneousCategory { get; }
    string? GetPriceOption(string? price);
    string? GetTradeIndexedOption(string? timeFrame);
}
