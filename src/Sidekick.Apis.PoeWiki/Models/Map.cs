using Sidekick.Apis.PoeWiki.Api;

namespace Sidekick.Apis.PoeWiki.Models
{
    public record Map
    {
        internal Map(MapResult map, List<BossResult>? bosses, List<ItemResult>? items, Uri? mapScreenshotUri)
        {
            Id = map.AreaId;
            Name = map.Name;
            Bosses = bosses?.Select(x => new Boss(x)).ToList();
            Drops = items?.Select(x => new ItemDrop(x)).ToList();
            Screenshot = mapScreenshotUri;
            AreaTypeTags = map.AreaTypeTags;
        }

        public string? Id { get; init; }

        public string? Name { get; init; }

        public Uri? Screenshot { get; init; }

        public List<Boss>? Bosses { get; init; }

        public List<ItemDrop>? Drops { get; init; }

        public List<string>? AreaTypeTags { get; init; }
    }
}
