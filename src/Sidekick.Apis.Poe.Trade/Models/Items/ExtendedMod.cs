namespace Sidekick.Apis.Poe.Trade.Models.Items;

public class ExtendedMod
{
    public string? Name { get; set; }
    public string? Tier { get; set; }
    public List<ExtendedModMagnitude>? Magnitudes { get; set; }
}
