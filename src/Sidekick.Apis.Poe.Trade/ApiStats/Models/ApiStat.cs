namespace Sidekick.Apis.Poe.Trade.ApiStats.Models;

public record ApiStat
{
    public required string Id { get; init; }
    public required string Text { get; set; }
    public required string Type { get; init; }

    public ApiStatOptions? Option { get; set; }
}
