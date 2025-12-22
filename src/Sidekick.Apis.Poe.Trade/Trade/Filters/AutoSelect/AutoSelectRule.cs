using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectRule
{
    [JsonPropertyName("checked")]
    public bool Checked { get; set; }

    [JsonPropertyName("conditions")]
    public List<AutoSelectCondition> Conditions { get; set; }
}
