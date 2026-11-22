using System.Text.Json.Serialization;

namespace Sidekick.Game.Ninja;

public class NinjaStashItemStat
{
    [JsonPropertyName("id")]
    public string? TradeStatId { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }

    public override string ToString()
    {
        return TradeStatId ?? string.Empty;
    }
}