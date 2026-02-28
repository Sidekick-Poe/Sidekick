using Sidekick.Data.Items.Models;

namespace Sidekick.Data.Trade.Models;

public class TradeStatDefinition
{
    public required StatCategory Category { get; init; }

    public required string Id { get; init; }

    public required string Text { get; init; }

    public List<TradeStatOption>? Options { get; init; } = [];
}
