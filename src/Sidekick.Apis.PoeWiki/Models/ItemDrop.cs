using Sidekick.Apis.PoeWiki.Api;

namespace Sidekick.Apis.PoeWiki.Models
{
    public record ItemDrop
    {
        internal ItemDrop(MapItemResult itemResult)
        {
            Name = itemResult.Name;
        }

        public string? Name { get; init; }
    }
}
