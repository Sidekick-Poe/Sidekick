using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Data.Items.Models;
namespace Sidekick.Apis.Poe.Trade.Trade.Items;

public interface IItemTradeService
{
    Task<TradeSearchResult<string>> Search(Item item, List<TradeFilter>? filters = null);

    Task<List<TradeResult>> GetResults(GameType game, string queryId, List<string> ids);

    Task<Uri> GetTradeUri(GameType game, string queryId);
}
