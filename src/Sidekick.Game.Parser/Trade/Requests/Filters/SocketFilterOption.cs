using System.Text.Json.Serialization;
using Sidekick.Game.Parser.Filters.Types;
namespace Sidekick.Game.Parser.Trade.Requests.Filters;

public class SocketFilterOption(SocketPropertyFilter filter)
{
    public double? Min { get; set; } = filter.Min;

    public double? Max { get; set; } = filter.Max;

    [JsonPropertyName("r")]
    public int? Red { get; set; } = filter.Red;

    [JsonPropertyName("g")]
    public int? Green { get; set; } = filter.Green;

    [JsonPropertyName("b")]
    public int? Blue { get; set; } = filter.Blue;

    [JsonPropertyName("w")]
    public int? White { get; set; } = filter.White;
}
