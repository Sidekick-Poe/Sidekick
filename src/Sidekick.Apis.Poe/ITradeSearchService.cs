using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface ITradeSearchService
    {
        Task<TradeSearchResult<string>> SearchBulk(Item item);
        Task<TradeSearchResult<string>> Search(Item item, PropertyFilters propertyFilters = null, ModifierFilters modifierFilters = null);
        Task<List<TradeItem>> GetResults(string queryId, List<string> ids, ModifierFilters modifierFilters = null);
        Uri GetTradeUri(Item item, string queryId);
    }
}