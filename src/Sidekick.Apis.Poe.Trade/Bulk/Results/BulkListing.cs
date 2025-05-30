namespace Sidekick.Apis.Poe.Trade.Bulk.Results;

public class BulkListing
{
    public BulkAccount? Account { get; set; }

    public DateTimeOffset Indexed { get; set; }

    public List<BulkOffer> Offers { get; set; } = new();
}
