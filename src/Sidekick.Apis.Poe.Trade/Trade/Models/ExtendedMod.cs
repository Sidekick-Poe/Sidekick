namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ExtendedMod
{
    public string? Name { get; set; }
    public string? Tier { get; set; }
    public List<ExtendedModMagnitude>? Magnitudes { get; set; }
}
