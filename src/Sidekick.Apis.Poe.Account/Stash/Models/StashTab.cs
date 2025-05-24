namespace Sidekick.Apis.Poe.Account.Stash.Models;

public class StashTab
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string League { get; init; }

    public required StashType Type { get; init; }
}
