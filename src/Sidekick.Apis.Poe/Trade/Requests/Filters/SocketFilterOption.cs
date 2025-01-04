using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

public class SocketFilterOption : StatFilterValue
{
    [JsonPropertyName("r")]
    public int? Red { get; set; }

    [JsonPropertyName("g")]
    public int? Green { get; set; }

    [JsonPropertyName("b")]
    public int? Blue { get; set; }

    [JsonPropertyName("w")]
    public int? White { get; set; }
}
