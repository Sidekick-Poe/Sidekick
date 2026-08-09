using Sidekick.Common.Initialization;
using Sidekick.Game.Items;
using Sidekick.Game.Stats;
using Sidekick.Game.StatsInvariant;
using Sidekick.Game.TradeStats;
using TradeFilter = Sidekick.Apis.Poe.Trade.Filters.Types.TradeFilter;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public interface IStatParser : IInitializableService
{
    StatsInvariantDetails InvariantDetails { get; }

    Dictionary<string, List<TradeStatDefinition>> TradeDefinitions { get; }

    void Parse(Item item);

    Stat? ParseInvariant(StatCategory category, string? line);

    Task<List<TradeFilter>> GetFilters(Item item);
}
