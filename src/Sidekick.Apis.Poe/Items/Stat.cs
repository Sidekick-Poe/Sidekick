using Sidekick.Data.Items;
using Sidekick.Data.Stats;
namespace Sidekick.Apis.Poe.Items;

/// <summary>
///     Represents a line of text on an item. With the API being the way it is, each line of text can be represented by one
///     or more api modifiers.
/// </summary>
public class Stat(StatCategory category, string text)
{
    /// <summary>
    ///     Gets or sets the original line of text as it is in the game.
    /// </summary>
    public string Text { get; } = text;

    public StatCategory Category { get; } = category;

    public List<StatDefinition> Definitions { get; set; } = [];

    public bool HasMultipleCategories
    {
        get
        {
            if (Definitions.Any(x => x.Category == StatCategory.Undefined)) return true;
            return Definitions.DistinctBy(x => x.Category).Count() > 1;
        }
    }

    /// <summary>
    ///     Gets or sets a list of values on this modifier line.
    /// </summary>
    public List<double> Values { get; set; } = [];

    /// <summary>
    /// Gets the average value of the numerical values associated with the modifier line.
    /// If there are no values, returns 0.
    /// </summary>
    public double AverageValue => Values.Count > 0 ? Values.Average() : 0;

    /// <summary>
    ///     Gets a value indicating whether this modifier has double values.
    /// </summary>
    public bool HasValues => Definitions.All(x => x.Option == null) && Values.Count > 0;

    public int BlockIndex { get; init; }

    public int LineIndex { get; init; }

    public bool MatchedFuzzily { get; init; }

    public bool HasTradeSupport { get; init; } = true;

    /// <inheritdoc />
    public override string ToString() => Text;
}
