namespace Sidekick.Apis.Poe.Trade.ApiStats.Models;

/// <summary>
/// Pseudo, Explicit, Implicit, etc.
/// </summary>
public class ApiCategory
{
    public string? Label { get; set; }

    public List<ApiStat> Entries { get; set; } = new();

    public override string ToString() => Label ?? string.Empty;
}
