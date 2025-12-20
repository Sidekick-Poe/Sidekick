using Sidekick.Apis.Poe.Trade.Trade.Bulk.Results;
namespace Sidekick.Apis.Poe.Trade.Trade.Bulk.Models;

public class BulkOfferModel
{
    public BulkOfferModel(BulkListing listing)
    {
        AccountName = listing.Account?.Name;
        AccountCharacter = listing.Account?.LastCharacterName;
        Date = listing.Indexed;

        var offer = listing.Offers.FirstOrDefault();
        if (offer == null)
        {
            return;
        }

        SaleAmount = offer.Exchange?.Amount ?? 0;
        SaleCurrency = offer.Exchange?.Currency;

        ItemAmount = offer.Item?.Amount ?? 0;
        ItemCurrency = offer.Item?.Currency;
        ItemStock = offer.Item?.Stock ?? 0;
    }

    public string? AccountName { get; }

    public string? AccountCharacter { get; }

    public DateTimeOffset Date { get; }

    public string? SaleCurrency { get; }

    public decimal SaleAmount { get; }

    public string? ItemCurrency { get; }

    public decimal ItemAmount { get; }

    public int ItemStock { get; }

    public decimal SaleUnitPrice => SaleAmount / ItemAmount;
}
