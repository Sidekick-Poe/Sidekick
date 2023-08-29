using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface ITradeSearchService
    {
        Task<TradeSearchResult<string>> SearchBulk(Item item);

        Task<TradeSearchResult<string>> Search(Item item, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null);

        Task<List<TradeItem>> GetResults(string queryId, List<string> ids, List<ModifierFilter>? modifierFilters = null);

        Uri GetTradeUri(Item item, string queryId);
    }
}
