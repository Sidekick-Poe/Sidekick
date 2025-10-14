using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade;

public interface ITradeSearchService
{
    Task<TradeSearchResult<string>> Search(Item item, List<PropertyFilter>? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoFilter>? pseudoFilters = null);

    Task<List<TradeResult>> GetResults(GameType game, string queryId, List<string> ids);

    Task<Uri> GetTradeUri(GameType game, string queryId);
}
