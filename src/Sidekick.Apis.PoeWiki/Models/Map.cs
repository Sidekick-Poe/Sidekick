using Sidekick.Apis.PoeWiki.Api;

namespace Sidekick.Apis.PoeWiki.Models
{
    public record Map
    {
        public Map(MapResult map, List<BossResult>? bosses, List<MapItemResult>? items)
        {
            Id = map.AreaId;
            Name = map.Name;
            Bosses = bosses?.Select(x => new Boss(x)).ToList();
            Drops = items?.Select(x => new ItemDrop(x)).ToList();
        }

        public string? Id { get; init; }

        public string? Name { get; init; }

        public List<Boss>? Bosses { get; init; }

        public List<ItemDrop>? Drops { get; init; }
    }
}
