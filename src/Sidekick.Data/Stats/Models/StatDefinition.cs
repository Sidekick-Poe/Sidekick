namespace Sidekick.Data.Stats.Models;

public class StatDefinition
{
    public List<string> GameIds { get; set; } = [];

    public List<GameStatPattern> GamePatterns { get; set; } = [];

    public List<TradeStatPattern> TradePatterns { get; set; } = [];
}
