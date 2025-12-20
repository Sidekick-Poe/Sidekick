using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade;

public interface ITradeSearchService
{
    Task<TradeSearchResult<string>> Search(Item item, List<TradeFilter>? propertyFilters = null, List<StatFilter>? statFilters = null, List<PseudoFilter>? pseudoFilters = null);

    Task<List<TradeResult>> GetResults(GameType game, string queryId, List<string> ids);

    Task<Uri> GetTradeUri(GameType game, string queryId);
}
