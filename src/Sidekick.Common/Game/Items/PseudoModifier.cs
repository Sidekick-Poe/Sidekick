namespace Sidekick.Common.Game.Items;

public class PseudoModifier(string text)
{
    public string? Id { get; init; }

    public string Text { get; set; } = text;

    public double Value { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this modifier has value.
    /// </summary>
    public bool HasValue => Value != 0;
}
