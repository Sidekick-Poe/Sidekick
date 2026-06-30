namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ExplicitModDetail
{
    public string? Name { get; set; }
    public string? Tier { get; set; }
    public List<Magnitude> Magnitudes { get; set; } = new();
}
