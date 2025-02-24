using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe;

public interface ITradeSearchService
{
    Task<TradeSearchResult<string>> Search(Item item, PropertyFilters? propertyFilters = null, IEnumerable<ModifierFilter>? modifierFilters = null, IEnumerable<PseudoModifierFilter>? pseudoFilters = null);

    Task<List<TradeItem>> GetResults(GameType game, string queryId, List<string> ids);

    Task<Uri> GetTradeUri(GameType game, string queryId);
}
