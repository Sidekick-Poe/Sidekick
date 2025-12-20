namespace Sidekick.Apis.Poe.Items;

public class PseudoStat
{
    public string? Id { get; init; }

    public required string Text { get; set; }

    public double Value { get; set; }
}
