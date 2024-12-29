namespace Sidekick.Common.Game.Items;

public class PseudoModifier
{
    public string? ModifierId { get; init; }

    /// <summary>
    /// Gets or sets a dictionary of modifiers. The key represents the modifier id from the API. The value represents its weighted sum value.
    /// </summary>
    public Dictionary<string, double> WeightedSumModifiers { get; init; } = [];

    public required string Text { get; set; }

    public double Value { get; set; }
}
