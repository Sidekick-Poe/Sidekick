namespace Sidekick.Apis.Poe.Trade.Parser;

/// <summary>
/// Stores data about each line in the parsing process
/// </summary>
public class ParsingLine
{
    /// <summary>
    /// Represents a line of text in the parsing process, including its content and position.
    /// </summary>
    public ParsingLine(string text, int index)
    {
        Text = text;
        Index = index;
    }

    /// <summary>
    /// Indicates if this line has been successfully parsed
    /// </summary>
    public bool Parsed { get; set; }

    /// <summary>
    /// The line of the item description
    /// </summary>
    public string Text { get; }

    public int Index { get; }

    public override string ToString()
    {
        return Text;
    }
}
