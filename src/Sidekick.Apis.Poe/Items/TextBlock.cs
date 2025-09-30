using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
namespace Sidekick.Apis.Poe.Items;

/// <summary>
/// Represents a single item section seperated by dashes when copying an item in-game.
/// </summary>
public partial class TextBlock
{
    [GeneratedRegex(@"[\r\n]+")]
    private static partial Regex NewLinePattern();

    /// <summary>
    /// Represents a section of an item description, separated by dashes, as part of the parsing process.
    /// </summary>
    public TextBlock(string text, int index)
    {
        Text = text;
        Index = index;

        Lines = NewLinePattern()
            .Split(Text)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select((x, lineIndex) => new TextLine(x, lineIndex))
            .ToList();
    }

    /// <summary>
    /// Contains all the lines inside this block
    /// </summary>
    public List<TextLine> Lines { get; set; }

    /// <summary>
    /// Indicates if this block has been successfully parsed by the parser
    /// </summary>
    public bool Parsed
    {
        get => Lines.All(x => x.Parsed);
        set
        {
            Lines.ForEach(x => x.Parsed = value);
        }
    }

    /// <summary>
    /// Indicates if this block has any of its lines parsed by the parser
    /// </summary>
    public bool AnyParsed => Lines.Any(x => x.Parsed);

    /// <summary>
    /// The text of the whole block
    /// </summary>
    public string Text { get; }

    public int Index { get; }

    public bool TryParseRegex(Regex pattern, [NotNullWhen(true)] out Match? match)
    {
        foreach (var line in Lines)
        {
            match = pattern.Match(line.Text);
            if (!match.Success)
            {
                continue;
            }

            line.Parsed = true;
            return true;
        }

        match = null;
        return false;
    }

    public override string ToString()
    {
        return Text;
    }
}
