namespace Sidekick.Apis.Poe.Static.Models;

/// <summary>
/// Currencies, Fragments, Maps, etc.
/// </summary>
public class StaticItemCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<StaticItem> Entries { get; set; } = new();
}
