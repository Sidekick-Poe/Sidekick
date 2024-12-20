using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface ITradeSearchService
    {
        Task<TradeSearchResult<string>> Search(Item item, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoModifierFilter>? pseudoFilters = null);

        Task<List<TradeItem>> GetResults(GameType game, string queryId, List<string> ids, List<PseudoModifierFilter>? pseudoFilters = null);

        Task<Uri> GetTradeUri(GameType game, string queryId);
    }
}
