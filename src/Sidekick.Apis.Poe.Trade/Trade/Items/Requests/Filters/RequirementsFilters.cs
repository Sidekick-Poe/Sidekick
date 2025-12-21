using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

public class RequirementsFilters
{
    [JsonPropertyName("lvl")]
    public StatFilterValue? Level { get; set; }

    [JsonPropertyName("str")]
    public StatFilterValue? Strength { get; set; }

    [JsonPropertyName("dex")]
    public StatFilterValue? Dexterity { get; set; }

    [JsonPropertyName("int")]
    public StatFilterValue? Intelligence { get; set; }
}
