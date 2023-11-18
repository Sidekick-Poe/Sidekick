using System.ComponentModel.DataAnnotations;

namespace Sidekick.Modules.Wealth.Models
{
    public class FullSnapshot
    {
        [Key]
        public DateTimeOffset Date { get; set; }

        public required string League { get; set; }

        public double Total { get; set; }
    }
}
