using Sidekick.Data.Items;
namespace Sidekick.Data.Trade;

public class TradeStatDefinition
{
    public required StatCategory Category { get; init; }

    public required string Id { get; init; }

    public required string Text { get; init; }

    public TradeStatOption? Option { get; init; }

    public override string ToString() => Text;
}
