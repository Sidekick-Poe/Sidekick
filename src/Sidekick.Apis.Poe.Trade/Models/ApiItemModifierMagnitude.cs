using System.Text.Json.Serialization;
using Sidekick.Common.Converters;
namespace Sidekick.Apis.Poe.Trade.Models;

public class ApiItemModifierMagnitude
{
    [JsonConverter(typeof(StringOrNumberConverter))]
    public string? Min { get; set; }

    [JsonConverter(typeof(StringOrNumberConverter))]
    public string? Max { get; set; }
}
