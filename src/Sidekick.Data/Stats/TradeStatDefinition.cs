namespace Sidekick.Data.Stats;

public class TradeStatDefinition
{
    public StatCategory Category { get; init; }

    public required string Id { get; init; }

    public required string Text { get; init; }

    public TradeStatOption? Option { get; init; }

    public override string ToString() => Text;
}
