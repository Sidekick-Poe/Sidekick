using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sidekick.Common.Database.Tables;

[PrimaryKey(nameof(ItemId), nameof(Index))]
public class WealthSparkline
{
    [Key]
    [MaxLength(64)]
    [ForeignKey(nameof(Item))]
    public required string ItemId { get; set; }

    [Key] public required int Index { get; set; }

    public decimal? Value { get; set; }


    public virtual WealthItem? Item { get; set; }
}