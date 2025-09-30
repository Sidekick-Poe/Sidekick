namespace Sidekick.Apis.Poe.Trade.Parser.Tokenizers;

public class ItemNameToken
{
    public ItemNameToken(ItemNameTokenType tokenType, ItemNameTokenMatch? value)
    {
        TokenType = tokenType;
        Match = value;
    }

    public ItemNameTokenType TokenType { get; set; }

    public ItemNameTokenMatch? Match { get; set; }
}
