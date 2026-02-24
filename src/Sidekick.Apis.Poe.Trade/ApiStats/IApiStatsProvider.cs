using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    List<TradeStatDefinition> Definitions { get; }

    TradeInvariantStats InvariantStats { get; }
}
