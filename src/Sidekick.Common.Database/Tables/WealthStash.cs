using System.ComponentModel.DataAnnotations;

namespace Sidekick.Common.Database.Tables
{
    public class WealthStash
    {
        [Key]
        [MaxLength(64)]
        public required string Id { get; set; }

        [MaxLength(64)]
        public string? Parent { get; set; }

        [MaxLength(64)]
        public required string Name { get; set; }

        [MaxLength(64)]
        public required string League { get; set; }

        [MaxLength(64)]
        public required string Type { get; set; }

        public decimal Total { get; set; }

        public DateTimeOffset LastUpdate { get; set; }
    }
}
