namespace Sidekick.Apis.Poe.Account.Stash.Models;

public class StashTabDetails
{
    public required string Id { get; set; }

    public string? Parent { get; set; }

    public required string Name { get; set; }

    public required string League { get; set; }

    public required StashType Type { get; set; }

    public List<StashItem> Items { get; set; } = new();
}
