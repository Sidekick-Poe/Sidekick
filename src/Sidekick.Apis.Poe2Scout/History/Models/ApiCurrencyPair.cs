using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiCurrencyPair
{
    [JsonPropertyName("CurrencyOneData")]
    public ApiCurrencyData? One { get; set; }

    [JsonPropertyName("CurrencyTwoData")]
    public ApiCurrencyData? Two { get; set; }
}
