using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiHistoryResult
{
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonPropertyName("price_history")]
    public List<ScoutHistoryLog> Logs { get; set; } = new();
}
