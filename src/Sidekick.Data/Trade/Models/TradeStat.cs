namespace Sidekick.Data.Trade.Models;

public record TradeStat
{
    public required string Id { get; init; }
    public required string Text { get; set; }
    public required string Type { get; init; }

    public TradeStatOptions? Option { get; set; }
}
