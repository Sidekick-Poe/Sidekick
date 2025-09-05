namespace Sidekick.Apis.Poe.Trade.Modifiers.Models;

/// <summary>
/// Pseudo, Explicit, Implicit, etc.
/// </summary>
public class ApiCategory
{
    public string? Label { get; set; }

    public List<ApiModifier> Entries { get; set; } = new();

    public override string ToString() => Label ?? string.Empty;
}
