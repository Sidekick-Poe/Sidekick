namespace Sidekick.Data.Trade.Models;

public class TradeFilterCategory
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public List<TradeFilter> Filters { get; set; } = new();

}
