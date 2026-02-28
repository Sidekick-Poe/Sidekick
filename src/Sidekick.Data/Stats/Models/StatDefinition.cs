namespace Sidekick.Data.Stats.Models;

public class StatDefinition
{
    public List<string> GameIds { get; set; } = [];

    public List<StatPattern> Patterns { get; set; } = [];
}
