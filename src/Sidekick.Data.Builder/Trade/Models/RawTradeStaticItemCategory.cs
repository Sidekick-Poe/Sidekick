namespace Sidekick.Data.Builder.Trade.Models;

public class RawTradeStaticItemCategory
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public List<RawTradeStaticItem> Entries { get; set; } = new();

    public override string ToString() => Label ?? string.Empty;
}
