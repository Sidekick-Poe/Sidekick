using Sidekick.Apis.PoeWiki.Api;

namespace Sidekick.Apis.PoeWiki.Models;

public record ItemDrop
{
    internal ItemDrop(ItemResult itemResult)
    {
        Name = itemResult.Name;
        Description = itemResult.Description;
    }

    public string? Name { get; init; }

    public string? Description { get; init; }
}
