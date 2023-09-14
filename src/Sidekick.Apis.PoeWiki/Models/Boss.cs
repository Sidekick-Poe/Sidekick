using Sidekick.Apis.PoeWiki.Api;

namespace Sidekick.Apis.PoeWiki.Models
{
    public record Boss
    {
        internal Boss(BossResult boss)
        {
            Id = boss.MetadataId;
            Name = boss.Name;
        }

        public string? Id { get; init; }

        public string? Name { get; init; }
    }
}
