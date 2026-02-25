namespace Sidekick.Data.Items;

public class ItemStatDefinition
{
    public List<string> GameIds { get; set; } = [];

    public List<ItemStatGamePattern> GamePatterns { get; set; } = [];

    public List<ItemStatTradePattern> TradePatterns { get; set; } = [];
}
