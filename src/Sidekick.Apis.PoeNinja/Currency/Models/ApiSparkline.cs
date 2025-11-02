namespace Sidekick.Apis.PoeNinja.Currency.Models;

public class ApiSparkline
{
    public decimal TotalChange { get; set; }

    public List<decimal?> Data { get; set; } = new();
}
