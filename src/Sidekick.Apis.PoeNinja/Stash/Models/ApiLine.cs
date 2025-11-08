using Sidekick.Apis.PoeNinja.Exchange.Models;
namespace Sidekick.Apis.PoeNinja.Stash.Models;

public class ApiLine
{
    public string? DetailsId { get; set; }
    public string? BaseType { get; set; }
    public string? Name { get; set; }
    public string? Variant { get; set; }

    public decimal ChaosValue { get; set; }
    public decimal DivineValue { get; set; }
    public decimal ExaltedValue { get; set; }
    public int ListingCount { get; set; }

    public bool? Corrupted { get; set; }
    public int? GemLevel { get; set; }
    public int? GemQuality { get; set; }
    public int? Links { get; set; }
    public int? LevelRequired { get; set; }
    public int? MapTier { get; set; }

    public ApiSparkline? Sparkline { get; set; }
}
