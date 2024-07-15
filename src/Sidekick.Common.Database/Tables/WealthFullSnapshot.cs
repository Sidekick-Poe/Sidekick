using System.ComponentModel.DataAnnotations;

namespace Sidekick.Common.Database.Tables
{
    public class WealthFullSnapshot
    {
        [Key]
        public DateTimeOffset Date { get; set; }

        [MaxLength(64)]
        public required string League { get; set; }

        public decimal Total { get; set; }
    }
}
