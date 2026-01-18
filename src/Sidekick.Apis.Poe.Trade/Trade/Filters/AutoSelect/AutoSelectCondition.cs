using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectCondition
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectConditionType Type { get; set; }

    [JsonPropertyName("comparison")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectComparisonType Comparison { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
