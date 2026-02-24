namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeStatCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<RawTradeStat> Entries { get; set; } = new();

    public override string ToString() => $"{Label} - {Entries.Count} entries";
}
