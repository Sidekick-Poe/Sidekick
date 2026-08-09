namespace Sidekick.Game.ItemDefinitions;

public class NinjaStashDefinition
{
    public required string Type { get; init; }
    public required string Url { get; init; }

    public string? DetailsId { get; init; }
    public bool Corrupted { get; init; }
    public bool Foulborn { get; init; }
    public int? GemLevel { get; init; }
    public int? GemQuality { get; init; }
    public int? Links { get; init; }
    public int? ItemLevel { get; init; }
    public string? Variant { get; init; }

    public List<NinjaStashStatDefinition>? Stats { get; init; }
}