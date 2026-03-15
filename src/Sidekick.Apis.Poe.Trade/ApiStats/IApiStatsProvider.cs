using Sidekick.Common.Initialization;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    StatsInvariantDetails InvariantDetails { get; }
}
