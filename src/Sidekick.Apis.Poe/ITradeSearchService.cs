using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface ITradeSearchService
    {
        Task<TradeSearchResult<string>> Search(Item item, TradeCurrency currency, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoModifierFilter>? pseudoFilters = null);

        Task<List<TradeItem>> GetResults(string queryId, List<string> ids, List<PseudoModifierFilter>? pseudoFilters = null);

        Task<Uri> GetTradeUri(string queryId);
    }
}
