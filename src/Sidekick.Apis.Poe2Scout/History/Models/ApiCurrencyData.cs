using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiCurrencyData
{
    [JsonPropertyName("CurrencyItemId")]
    public int CurrencyItemId { get; set; }

    [JsonPropertyName("RelativePrice")]
    public string? _RelativePrice { get; set; }

    [JsonPropertyName("VolumeTraded")]
    public int VolumeTraded { get; set; }

    [JsonIgnore]
    public decimal RelativePrice => decimal.TryParse(_RelativePrice, out var result) ? result : 0;
}
