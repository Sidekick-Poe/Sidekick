using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    List<TradeStatDefinition> Definitions { get; }

    Dictionary<string, List<TradeStatDefinition>> IdDictionary { get; }

    TradeInvariantStats InvariantStats { get; }
}
