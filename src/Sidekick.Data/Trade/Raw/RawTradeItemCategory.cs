namespace Sidekick.Data.Trade.Raw;

public class RawTradeItemCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<RawTradeItem> Entries { get; set; } = new();

    public override string ToString() => $"{Label} - {Entries.Count} entries";
}
