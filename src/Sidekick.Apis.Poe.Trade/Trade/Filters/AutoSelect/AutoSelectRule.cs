using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectRule
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonPropertyName("conditions")]
    public List<AutoSelectCondition> Conditions { get; set; } = [];

    [JsonPropertyName("checked")]
    public bool? Checked { get; set; }

    [JsonPropertyName("selectedValue")]
    public string? SelectedValue { get; set; }

    [JsonPropertyName("fillMin")]
    public bool? FillMinRange { get; set; }

    [JsonPropertyName("fillMax")]
    public bool? FillMaxRange { get; set; }

    [JsonPropertyName("normalizeBy")]
    public double? NormalizeBy { get; set; }

}