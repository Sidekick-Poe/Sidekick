namespace Sidekick.Apis.Poe2Scout.Api;

public record Poe2ScoutPriceLog
{
    public decimal? Price { get; init; }
    public int? Quantity { get; init; }
    public DateTimeOffset? Time { get; init; }
}
