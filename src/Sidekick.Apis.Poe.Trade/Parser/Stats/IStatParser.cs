using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public interface IStatParser : IInitializableService
{
    StatsInvariantDetails InvariantDetails { get; }

    void Parse(Item item);

    Stat? ParseInvariant(string? line);

    Task<List<TradeFilter>> GetFilters(Item item);
}
