namespace Sidekick.Apis.Poe.Items;

/// <summary>
///     Represents a line of text on an item. With the API being the way it is, each line of text can be represented by one
///     or more api modifiers.
/// </summary>
public class Stat(string text)
{
    /// <summary>
    ///     Gets or sets the original line of text as it is in the game.
    /// </summary>
    public string Text { get; } = text;

    /// <summary>
    ///     Gets or sets the modifier associated with this line.
    /// </summary>
    public List<StatApiInformation> ApiInformation { get; } = [];

    /// <summary>
    ///     Gets or sets a list of values on this modifier line.
    /// </summary>
    public List<double> Values { get; } = [];

    /// <summary>
    /// Gets the average value of the numerical values associated with the modifier line.
    /// If there are no values, returns 0.
    /// </summary>
    public double AverageValue => Values.Count > 0 ? Values.Average() : 0;

    /// <summary>
    ///     Gets or sets the option value of this modifier.
    /// </summary>
    public int? OptionValue { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this modifier has double values.
    /// </summary>
    public bool HasValues => OptionValue == null && Values.Count > 0;

    public int BlockIndex { get; init; }

    public int LineIndex { get; init; }

    public bool MatchedFuzzily { get; init; }

    /// <inheritdoc />
    public override string ToString() => Text;
}
