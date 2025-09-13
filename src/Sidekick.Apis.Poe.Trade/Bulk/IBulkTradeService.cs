using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade.Bulk.Models;

namespace Sidekick.Apis.Poe.Trade.Bulk;

public interface IBulkTradeService
{
    Task<BulkResponseModel> SearchBulk(Item item);

    bool SupportsBulkTrade(Item? item);

    Task<Uri> GetTradeUri(Item item, string queryId);
}
