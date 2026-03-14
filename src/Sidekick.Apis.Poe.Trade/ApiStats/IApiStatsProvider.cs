using Sidekick.Common.Initialization;
using Sidekick.Data.Trade;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    TradeInvariantStats InvariantStats { get; }
}
