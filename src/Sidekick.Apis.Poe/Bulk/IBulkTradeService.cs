using Sidekick.Apis.Poe.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Bulk
{
    public interface IBulkTradeService
    {
        Task<BulkResponseModel> SearchBulk(Item item, TradeCurrency currency, int minStock);

        bool SupportsBulkTrade(Item? item);

        Uri GetTradeUri(Item item, string queryId);
    }
}
