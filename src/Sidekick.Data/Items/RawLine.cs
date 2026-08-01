using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Items;

/// <summary>
/// Stores data about each line in the parsing process
/// </summary>
public class RawLine(string text, int index)
{
    /// <summary>
    /// Indicates if this line has been successfully parsed
    /// </summary>
    public bool Parsed { get; set; }

    /// <summary>
    /// The line of the item description
    /// </summary>
    public string Text { get; } = text;

    public int Index { get; } = index;

    public bool TryParseRegex(Regex pattern, [NotNullWhen(true)] out Match? match)
    {
        match = pattern.Match(Text);
        if (!match.Success) return false;

        Parsed = true;
        return true;
    }

    public override string ToString()
    {
        return Text;
    }
}
