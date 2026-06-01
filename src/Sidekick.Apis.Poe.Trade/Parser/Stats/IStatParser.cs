using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Common.Initialization;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Items;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public interface IStatParser : IInitializableService
{
    StatsInvariantDetails InvariantDetails { get; }

    Dictionary<string, TradeStatDefinition> TradeDefinitions { get; }

    void Parse(Item item);

    Stat? ParseInvariant(string? line);

    Task<List<TradeFilter>> GetFilters(Item item);
}
