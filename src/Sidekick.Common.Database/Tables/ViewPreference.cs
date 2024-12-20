using System.ComponentModel.DataAnnotations;

namespace Sidekick.Common.Database.Tables;

public class ViewPreference
{
    [Key]
    [MaxLength(64)]
    public required string Key { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}
