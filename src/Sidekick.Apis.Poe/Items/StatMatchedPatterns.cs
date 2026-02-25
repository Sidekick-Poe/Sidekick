using Sidekick.Data.Stats.Models;
namespace Sidekick.Apis.Poe.Items;

public class StatMatchedPatterns(string text)
{
    public required ItemStatDefinition Definition { get; set; }

    public ItemStatGamePattern? GamePattern { get; set; }

    public ItemStatTradePattern? TradePattern { get; set; }
}
