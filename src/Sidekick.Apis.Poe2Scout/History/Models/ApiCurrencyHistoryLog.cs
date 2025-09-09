using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ApiCurrencyHistoryLog
{
    [JsonPropertyName("Epoch")]
    public int Epoch { get; set; }

    [JsonPropertyName("Data")]
    public ApiCurrencyPair? Pair { get; set; }

    [JsonIgnore]
    public decimal Value
    {
        get
        {
            if (Pair?.One?.VolumeTraded > 0 && Pair?.Two?.VolumeTraded > 0)
            {
                return (decimal)Pair.Two.VolumeTraded / Pair.One.VolumeTraded;
            }

            return 0;
        }
    }

    [JsonIgnore]
    public int Quantity => Pair?.One?.VolumeTraded ?? 0;

    [JsonIgnore]
    public DateTimeOffset DateTime => DateTimeOffset.FromUnixTimeSeconds(Epoch);

}
