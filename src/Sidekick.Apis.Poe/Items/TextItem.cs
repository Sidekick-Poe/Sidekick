using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Trade.Parser.Tokenizers;
namespace Sidekick.Apis.Poe.Items;

/// <summary>
/// Stores data about the state of the parsing process for the item
/// </summary>
public class TextItem
{
    public const string SeparatorPattern = "--------";

    /// <summary>
    /// Stores data about the state of the parsing process for the item
    /// </summary>
    /// <param name="text">The original text of the item</param>
    public TextItem(string text)
    {
        Text = new ItemNameTokenizer().CleanString(text);
        Text = Text.RemoveSquareBrackets();
        Blocks = Text.Split(SeparatorPattern, StringSplitOptions.RemoveEmptyEntries).Select((x, blockIndex) => new TextBlock(x.Trim('\r', '\n'), blockIndex)).ToList();
    }

    /// <summary>
    /// Item sections seperated by dashes when copying an item in-game.
    /// </summary>
    public List<TextBlock> Blocks { get; }

    /// <summary>
    /// The original text of the item
    /// </summary>
    public string Text { get; }

    public bool TryParseRegex(Regex pattern, [NotNullWhen(true)] out Match? match)
    {
        foreach (var block in Blocks)
        {
            if (block.TryParseRegex(pattern, out match))
            {
                return true;
            }
        }

        match = null;
        return false;
    }

    public override string ToString()
    {
        return Text;
    }
}
