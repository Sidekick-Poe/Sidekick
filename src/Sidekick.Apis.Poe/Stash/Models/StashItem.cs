using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Stash.Models;

public class StashItem
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Stash { get; set; }
    public required string League { get; set; }
    public string? Icon { get; set; }
    public int? ItemLevel { get; set; }
    public int? MapTier { get; set; }
    public int? GemLevel { get; set; }
    public int? MaxLinks { get; set; }
    public Category Category { get; set; }
    public required int Count { get; set; }
}
