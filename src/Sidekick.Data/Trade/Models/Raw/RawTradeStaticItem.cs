namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeStaticItem
{
    public required string Id { get; set; }
    public string? Text { get; set; }
    public string? Image { get; set; }

    public override string ToString() => Text ?? string.Empty;
}
