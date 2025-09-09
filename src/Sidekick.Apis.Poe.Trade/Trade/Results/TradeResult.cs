using Sidekick.Apis.Poe.Trade.Models.Items;

namespace Sidekick.Apis.Poe.Trade.Trade.Results;

public class TradeResult
{
    public required string Id { get; init; }
    public required Listing Listing { get; init; }
    public required ApiItem Item { get; init; }
}
