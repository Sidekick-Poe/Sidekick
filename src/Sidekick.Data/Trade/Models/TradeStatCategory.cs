namespace Sidekick.Data.Trade.Models;

public class TradeStatCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<TradeStat> Entries { get; set; } = new();

    public override string ToString() => $"{Label} - {Entries.Count} entries";
}
