namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class NinjaSparkline
{
    public decimal TotalChange { get; set; }

    public List<decimal?> Data { get; set; } = [];
}
