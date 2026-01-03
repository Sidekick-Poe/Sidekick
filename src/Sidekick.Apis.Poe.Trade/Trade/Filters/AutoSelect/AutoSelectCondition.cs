using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

#pragma warning disable CS0659

public class AutoSelectCondition : IEquatable<AutoSelectCondition>
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectConditionType Type { get; set; }

    [JsonPropertyName("comparison")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectComparisonType Comparison { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    public bool Equals(AutoSelectCondition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Comparison == other.Comparison && Equals(Value, other.Value) && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AutoSelectCondition)obj);
    }
}
