using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Sidekick.Modules.Wealth.Models
{
    [PrimaryKey(nameof(Date), nameof(StashId))]
    public class StashSnapshot
    {
        [Key]
        public DateTimeOffset Date { get; set; }

        [Key]
        public required string StashId { get; set; }

        public required string League { get; set; }

        public double Total { get; set; }
    }
}
