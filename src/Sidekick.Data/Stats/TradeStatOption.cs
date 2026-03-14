namespace Sidekick.Data.Stats;

public class TradeStatOption
{
    public required int Id { get; init; }
    public required string Text { get; init; }

    public override string ToString() => Text;
}
