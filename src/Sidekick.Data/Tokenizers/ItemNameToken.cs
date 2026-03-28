namespace Sidekick.Apis.Poe.Trade.Parser.Tokenizers;

public class ItemNameToken(ItemNameTokenType tokenType, ItemNameTokenMatch? value)
{

    public ItemNameTokenType TokenType { get; set; } = tokenType;

    public ItemNameTokenMatch? Match { get; set; } = value;
}
