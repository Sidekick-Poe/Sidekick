using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Game.ItemDefinitions;
using Sidekick.Game.Ninja;
namespace Sidekick.Apis.PoeNinja.Stash;

public class NinjaStash
{
    public NinjaStash(ApiStashLine line, ApiStashOverview result)
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

    public ApiSparkline? Sparkline { get; set; }

    public required Uri? DetailsUrl { get; set; }

    public required NinjaStashItem Item { get; set; }
}
