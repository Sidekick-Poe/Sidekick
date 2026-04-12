namespace Sidekick.Data.ItemDefinitions;

public class NinjaStashDefinition
{
    public string? DetailsId { get; init; }
    public string? Name { get; init; }
    public string? BaseType { get; init; }
    public bool? Corrupted { get; init; }
    public bool? Foulborn { get; set; }
    public int? GemLevel { get; init; }
    public int? GemQuality { get; init; }
    public int? Links { get; init; }
    public int? ItemLevel { get; init; }
    public string? Variant { get; init; }

    public List<NinjaStashStatDefinition>? Stats { get; init; }
}