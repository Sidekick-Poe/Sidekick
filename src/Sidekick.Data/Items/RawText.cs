using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sidekick.Data.Extensions;
using Sidekick.Data.Languages;
using Sidekick.Data.Tokenizers;
namespace Sidekick.Data.Items;

/// <summary>
/// Stores data about the state of the parsing process for the item
/// </summary>
public class RawText
{
    public const string SeparatorPattern = "--------";

    /// <summary>
    /// Represents the raw text of an item, storing its original text and parsed blocks of data.
    /// </summary>
    public RawText(IGameLanguage language, string text)
    {
        Text = new ItemNameTokenizer().CleanString(text);
        Text = Text.RemoveSquareBrackets();
        Blocks = Text.Split(SeparatorPattern, StringSplitOptions.RemoveEmptyEntries).Select((x, blockIndex) => new RawBlock(language, x.Trim('\r', '\n'), blockIndex)).ToList();
    }

    /// <summary>
    /// Item sections seperated by dashes when copying an item in-game.
    /// </summary>
    public List<RawBlock> Blocks { get; }

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
