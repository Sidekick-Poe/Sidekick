namespace Sidekick.Apis.PoeNinja.Stash.Models;

public class ApiSparkline
{
    public decimal TotalChange { get; set; }

    public List<decimal?> Data { get; set; } = new();
}
