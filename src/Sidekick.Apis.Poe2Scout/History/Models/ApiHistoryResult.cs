using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiHistoryResult
{
    public bool HasMore { get; set; }

    [JsonPropertyName("PriceHistory")]
    public List<ScoutHistoryLog> Logs { get; set; } = new();
}
