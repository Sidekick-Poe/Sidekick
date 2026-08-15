using System.Text.RegularExpressions;
namespace Sidekick.Game.Parser.Tokenizers;

public class ItemNameTokenMatch
{
    public ItemNameTokenMatch(
        ItemNameTokenType tokenType,
        Match match)
    {
        TokenType = tokenType;
        Match = match;
    }

    public ItemNameTokenType TokenType { get; set; }

    public Match Match { get; set; }
}
