using System.ComponentModel.DataAnnotations;

namespace Sidekick.Common.Database.Tables
{
    public class WealthItem
    {
        [Key]
        [MaxLength(64)]
        public required string Id { get; set; }

        [MaxLength(64)]
        public required string StashId { get; set; }

        [MaxLength(64)]
        public required string League { get; set; }

        [MaxLength(128)]
        public required string Name { get; set; }

        [MaxLength(64)]
        public required string Category { get; set; }

        [MaxLength(64)]
        public string? Icon { get; set; }

        public int? ItemLevel { get; set; }

        public int? MapTier { get; set; }

        public int? GemLevel { get; set; }

        public int? MaxLinks { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }
    }
}
