namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ApiItemModifierDetail
{
    public string? Name { get; set; }
    public string? Tier { get; set; }
    public List<ApiItemModifierMagnitude> Magnitudes { get; set; } = new();
}
