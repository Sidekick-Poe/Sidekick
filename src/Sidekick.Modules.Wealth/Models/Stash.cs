using System.ComponentModel.DataAnnotations;
using Sidekick.Apis.Poe.Stash.Models;

namespace Sidekick.Modules.Wealth.Models
{
    public class Stash
    {
        [Key]
        public required string Id { get; set; }

        public string? Parent { get; set; }

        public required string Name { get; set; }

        public required string League { get; set; }

        public required StashType Type { get; set; }

        public double Total { get; set; }

        public DateTimeOffset LastUpdate { get; set; }
    }
}
