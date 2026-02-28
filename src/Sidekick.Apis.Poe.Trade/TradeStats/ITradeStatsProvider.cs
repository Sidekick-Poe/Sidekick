using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;
namespace Sidekick.Apis.Poe.Trade.TradeStats;

public interface ITradeStatsProvider : IInitializableService
{
    Dictionary<string, TradeStatDefinition> Definitions { get; }

    TradeInvariantStats InvariantStats { get; }
}
