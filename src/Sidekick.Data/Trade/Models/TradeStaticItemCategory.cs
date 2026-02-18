namespace Sidekick.Data.Trade.Models;

public class TradeStaticItemCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<TradeStaticItem> Entries { get; set; } = new();

    public override string ToString() => Label ?? string.Empty;
}
