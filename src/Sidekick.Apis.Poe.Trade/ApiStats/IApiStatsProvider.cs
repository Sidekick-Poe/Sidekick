using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    Dictionary<StatKey, TradeStatDefinition> Definitions { get; }

    TradeInvariantStats InvariantStats { get; }
}
