using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
using Sidekick.Data.Trade;
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
