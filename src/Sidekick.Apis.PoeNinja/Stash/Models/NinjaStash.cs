using Sidekick.Apis.PoeNinja.Exchange.Models;
namespace Sidekick.Apis.PoeNinja.Stash.Models;

public class NinjaStash
{
    public NinjaStash(ApiLine line, ApiOverviewResult result)
    {
        LastUpdated = result.LastUpdated;
        ChaosValue = line.ChaosValue;
        DivineValue = line.DivineValue;
        ExaltedValue = line.ExaltedValue;
        ListingCount = line.ListingCount;
        Sparkline = line.SparkLine;
    }

    public DateTimeOffset LastUpdated { get; set; }

    public decimal ChaosValue { get; set; }
    public decimal DivineValue { get; set; }
    public decimal ExaltedValue { get; set; }
    public int ListingCount { get; set; }

    public ApiSparkline? Sparkline { get; set; }

    public required Uri? DetailsUrl { get; set; }
}
