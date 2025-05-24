using System.Text.Json.Serialization;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

public class StatFilterGroup
{
    [JsonIgnore]
    public StatType Type { get; set; }

    [JsonPropertyName("type")]
    public string? TypeAsString => Type.GetValueAttribute() ?? StatType.And.GetValueAttribute();

    public List<StatFilters> Filters { get; set; } = new();

    public StatFilterValue? Value { get; set; }
}
