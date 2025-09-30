namespace Sidekick.Apis.Poe.Items;

/// <summary>
/// Stores data about each line in the parsing process
/// </summary>
public class TextLine(string text, int index)
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

    public override string ToString()
    {
        return Text;
    }
}
