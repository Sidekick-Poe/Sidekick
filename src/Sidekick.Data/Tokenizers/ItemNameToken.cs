namespace Sidekick.Data.Tokenizers;

public class ItemNameToken(ItemNameTokenType tokenType, ItemNameTokenMatch? value)
{

    public ItemNameTokenType TokenType { get; set; } = tokenType;

    public ItemNameTokenMatch? Match { get; set; } = value;
}
