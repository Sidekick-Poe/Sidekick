namespace Sidekick.Apis.Poe.Items;

public class PseudoModifier
{
    public string? ModifierId { get; init; }

    public required string Text { get; set; }

    public double Value { get; set; }
}
