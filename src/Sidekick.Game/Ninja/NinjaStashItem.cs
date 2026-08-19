namespace Sidekick.Game.Ninja;

public class NinjaStashItem
{
    public required string Type { get; init; }
    public required string Url { get; init; }
    public required string DetailsId { get; init; }

    public bool Corrupted { get; init; }
    public bool Foulborn { get; init; }
    public int? GemLevel { get; init; }
    public int? GemQuality { get; init; }
    public int? Links { get; init; }
    public int? ItemLevel { get; init; }
    public int? MapTier { get; init; }
    public string? Variant { get; init; }

    public List<NinjaStashItemStat>? Stats { get; set; }

    public override string ToString()
    {
        return DetailsId;
    }
}