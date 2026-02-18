namespace Sidekick.Data.Trade.Models;

public class TradeItemCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<TradeItem> Entries { get; set; } = new();

    public override string ToString() => $"{Label} - {Entries.Count} entries";
}
