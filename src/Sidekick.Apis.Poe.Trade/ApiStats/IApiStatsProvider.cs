using Sidekick.Common.Initialization;
using Sidekick.Data.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    bool IsMatch(string id, string text);

    List<TradeStatDefinition> Definitions { get; }
}
