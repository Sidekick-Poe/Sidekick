using System.Text.Json.Serialization;

namespace Sidekick.Common.Game.Items;

public class DamageRange
{
    [JsonPropertyName("min")]
    public double Min { get; set; }

    [JsonPropertyName("max")]
    public double Max { get; set; }

    public static implicit operator (double Min, double Max)(DamageRange range) => (range.Min, range.Max);
    
    public static implicit operator DamageRange((double Min, double Max) tuple) => new DamageRange { Min = tuple.Min, Max = tuple.Max };
} 