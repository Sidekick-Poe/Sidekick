using Sidekick.Apis.Poe.Bulk.Results;

namespace Sidekick.Apis.Poe.Bulk.Models
{
    public class BulkResponseModel
    {
        public BulkResponseModel(BulkResponse response)
        {
            QueryId = response.Id;

            TotalOffers = response.Total;

            Offers = response.Result?.Values
                .Where(x => x.Listing != null)
                .ToList()
                .ConvertAll(x => new BulkOfferModel(x.Listing!))
                .OrderBy(x => x.SaleUnitPrice)
                .ToList() ?? new();
        }

        public string? QueryId { get; }

        public int TotalOffers { get; }

        public List<BulkOfferModel> Offers { get; }
    }
}
