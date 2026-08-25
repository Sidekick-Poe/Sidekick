using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiCurrencyHistoryLog
{
    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("Time")]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }
}
