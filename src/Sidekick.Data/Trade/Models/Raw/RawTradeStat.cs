namespace Sidekick.Data.Trade.Models.Raw;

public record RawTradeStat
{
    public required string Id { get; init; }
    public required string Text { get; set; }
    public required string Type { get; init; }

    public RawTradeStatOptions? Option { get; set; }
}
