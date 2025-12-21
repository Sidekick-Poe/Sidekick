using Sidekick.Apis.Poe.Trade.Trade.Items.Models;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Results;

public class TradeResult
{
    public required string Id { get; init; }
    public required Listing Listing { get; init; }
    public required ApiItem Item { get; init; }
}
