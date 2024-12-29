namespace Sidekick.Common.Game.Items;

public class PseudoModifier
{
    /// <summary>
    /// Gets or sets a dictionary of modifiers. The key represents the modifier id from the API. The value represents its weighted sum value.
    /// </summary>
    public Dictionary<string, double> Modifiers { get; init; } = [];

    public required string Text { get; set; }

    public double Value { get; set; }
}
