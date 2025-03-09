using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Parser.Properties.Filters;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

public class SocketFilterOption : StatFilterValue
{
    public SocketFilterOption() {}

    public SocketFilterOption(IntPropertyFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
        Min = filter.Min;
        Max = filter.Max;
    }

    [JsonPropertyName("r")]
    public int? Red { get; set; }

    [JsonPropertyName("g")]
    public int? Green { get; set; }

    [JsonPropertyName("b")]
    public int? Blue { get; set; }

    [JsonPropertyName("w")]
    public int? White { get; set; }
}
