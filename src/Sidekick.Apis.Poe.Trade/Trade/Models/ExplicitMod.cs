namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ExplicitMod
{
    public string? Description { get; set; }
    public string? Hash { get; set; }
    public List<ExplicitModDetail> Mods { get; set; } = new();
}
