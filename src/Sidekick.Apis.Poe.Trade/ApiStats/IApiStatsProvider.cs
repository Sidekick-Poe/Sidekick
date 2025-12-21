using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.ApiStats;

public interface IApiStatsProvider : IInitializableService
{
    StatCategory GetStatCategory(string apiId);

    bool IsMatch(string id, string text);

    Dictionary<StatCategory, List<StatDefinition>> Definitions { get; }
}
