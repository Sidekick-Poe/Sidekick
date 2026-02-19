namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeFilterCategory
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public List<RawTradeFilter> Filters { get; set; } = new();

}
