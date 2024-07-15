using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Sidekick.Common.Database.Tables
{
    [PrimaryKey(nameof(Date), nameof(StashId))]
    public class WealthStashSnapshot
    {
        [Key]
        public DateTimeOffset Date { get; set; }

        [Key]
        [MaxLength(64)]
        public required string StashId { get; set; }

        [MaxLength(64)]
        public required string League { get; set; }

        public decimal Total { get; set; }
    }
}
