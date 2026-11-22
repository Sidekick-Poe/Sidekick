using System.Text.Json.Serialization;

namespace Sidekick.Game.Ninja;

public class NinjaStashItem
{
    public bool Active { get; set; }

    public required string DetailsId { get; init; }

    public required string Type { get; init; }

    public string? Url { get; set; }

    public string? Name { get; set; }

    public string? BaseType { get; set; }

    [JsonIgnore]
    public bool Foulborn => Name?.StartsWith("Foulborn ", StringComparison.InvariantCultureIgnoreCase) ?? false;

    [JsonIgnore]
    public bool Blighted => Name?.StartsWith("Blighted ", StringComparison.InvariantCultureIgnoreCase) ?? false;

    [JsonIgnore]
    public bool BlightRavaged => Name?.StartsWith("Blight-ravaged ", StringComparison.InvariantCultureIgnoreCase) ?? false;

    public bool? Corrupted { get; set; }

    public int? GemLevel { get; set; }

    public int? GemQuality { get; set; }

    public int? Links { get; set; }

    public int? ItemLevel { get; set; }

    public int? MapTier { get; set; }

    public string? Variant { get; set; }

    public List<NinjaStashItemStat>? Stats { get; set; }

    public override string ToString()
    {
        return DetailsId;
    }
}
