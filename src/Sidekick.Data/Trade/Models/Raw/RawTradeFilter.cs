namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeFilter
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public RawTradeFilterOptions Option { get; set; } = new();
}
