using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiCurrencyHistoryResult
{
    [JsonPropertyName("History")]
    public List<ApiCurrencyHistoryLog> History { get; set; } = [];
}
