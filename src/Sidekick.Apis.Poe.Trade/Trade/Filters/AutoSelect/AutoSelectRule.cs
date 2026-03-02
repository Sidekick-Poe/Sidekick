using System.Text.Json.Serialization;
using Sidekick.Data.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectRule
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonPropertyName("conditions")]
    public List<AutoSelectCondition> Conditions { get; set; } = [];

    [JsonPropertyName("checked")]
    public bool? Checked { get; set; }

    [JsonPropertyName("fillMin")]
    public bool? FillMinRange { get; set; }

    [JsonPropertyName("fillMax")]
    public bool? FillMaxRange { get; set; }

    [JsonPropertyName("normalizeBy")]
    public double? NormalizeBy { get; set; }

    [JsonPropertyName("selectCategories")]
    public List<StatCategory>? SelectCategories { get; set; }

}