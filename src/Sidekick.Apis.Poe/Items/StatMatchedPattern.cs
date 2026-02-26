using Sidekick.Data.Stats.Models;
namespace Sidekick.Apis.Poe.Items;

public class StatMatchedPattern
{
    public required StatDefinition Definition { get; set; }

    public GameStatPattern? GamePattern { get; set; }

    public TradeStatPattern? TradePattern { get; set; }
}
