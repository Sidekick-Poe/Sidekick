using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface ITradeSearchService
    {
        Task<TradeSearchResult<string>> SearchBulk(Item item, TradeOptions options);

        Task<TradeSearchResult<string>> Search(Item item, TradeOptions options, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoModifierFilter>? pseudoFilters = null);

        Task<List<TradeItem>> GetResults(string queryId, List<string> ids, List<PseudoModifierFilter>? pseudoFilters = null);

        Uri GetTradeUri(Item item, string queryId);
    }
}
