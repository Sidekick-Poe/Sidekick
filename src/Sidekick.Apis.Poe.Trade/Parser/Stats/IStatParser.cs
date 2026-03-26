using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public interface IStatParser : IInitializableService
{
    StatsInvariantDetails InvariantDetails { get; }

    Dictionary<string, TradeStatDefinition> TradeDictionary { get; }

    string GetDictionaryKey(string id, string? option);

    void Parse(Item item);

    Task<List<TradeFilter>> GetFilters(Item item);
}
