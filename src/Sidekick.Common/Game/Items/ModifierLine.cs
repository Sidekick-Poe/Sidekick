namespace Sidekick.Common.Game.Items;

/// <summary>
///     Represents a line of text on an item. With the API being the way it is, each line of text can be represented by one
///     or more api modifiers.
/// </summary>
public class ModifierLine(string text)
{
    /// <summary>
    ///     Gets or sets the original line of text as it is in the game.
    /// </summary>
    public string Text { get; set; } = text;

    /// <summary>
    ///     Gets or sets a value indicating whether the modifier was found using a fuzzy search.
    /// </summary>
    public bool IsFuzzy { get; set; }

    /// <summary>
    ///     Gets or sets the modifier associated with this line.
    /// </summary>
    public List<Modifier> Modifiers { get; init; } = [];

    /// <summary>
    ///     Gets or sets a list of values on this modifier line.
    /// </summary>
    public List<double> Values { get; set; } = [];

    /// <summary>
    ///     Gets or sets the option value of this modifier.
    /// </summary>
    public int? OptionValue { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this modifier has double values.
    /// </summary>
    public bool HasValues => OptionValue == null && Values.Count > 0;

    /// <inheritdoc />
    public override string ToString() => Text;
}
