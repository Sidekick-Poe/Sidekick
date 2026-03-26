using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStash
{
    public NinjaStash(NinjaStashLine line, NinjaStashOverview result)
    {
        DetailsId = line.DetailsId;
        LastUpdated = result.LastUpdated;
        ChaosValue = line.ChaosValue;
        DivineValue = line.DivineValue;
        ExaltedValue = line.ExaltedValue;
        ListingCount = line.ListingCount;
        Sparkline = line.SparkLine;
    }

    public string? DetailsId { get; set; }

    public DateTimeOffset LastUpdated { get; set; }

    public decimal ChaosValue { get; set; }
    public decimal DivineValue { get; set; }
    public decimal ExaltedValue { get; set; }
    public int ListingCount { get; set; }

    public NinjaSparkline? Sparkline { get; set; }

    public required Uri? DetailsUrl { get; set; }

    public required NinjaItemDefinition Definition { get; set; }
}
