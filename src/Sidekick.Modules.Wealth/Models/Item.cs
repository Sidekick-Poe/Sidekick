using System.ComponentModel.DataAnnotations;
using Sidekick.Common.Game.Items;

namespace Sidekick.Modules.Wealth.Models
{
    public partial class Item
    {
        [Key]
        public required string Id { get; set; }

        public required string StashId { get; set; }

        public required string League { get; set; }

        public required string Name { get; set; }

        public Category Category { get; set; }

        public string? Icon { get; set; }

        public int? ItemLevel { get; set; }

        public int? MapTier { get; set; }

        public int? GemLevel { get; set; }

        public int? MaxLinks { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }

        public double Total { get; set; }
    }
}
