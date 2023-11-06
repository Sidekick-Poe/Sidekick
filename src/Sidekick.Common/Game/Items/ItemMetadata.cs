namespace Sidekick.Common.Game.Items
{
    public class ItemMetadata
    {
        public required string Id { get; init; }

        public required string? Name { get; set; }

        public required string? Type { get; init; }

        public required Rarity Rarity { get; set; }

        public required Category Category { get; init; }
    }
}
