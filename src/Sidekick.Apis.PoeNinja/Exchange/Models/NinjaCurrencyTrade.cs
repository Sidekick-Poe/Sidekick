namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class NinjaCurrencyTrade
{
    public NinjaCurrencyTrade(ApiLine line, ApiCore core, string? exchangeId)
    {
        decimal rate = 1;
        if (exchangeId != null && core.Rates.TryGetValue(exchangeId, out var coreRate)) rate = coreRate;

        ExchangeId = exchangeId;
        Value = line.PrimaryValue * rate;
        Volume = core.Primary == exchangeId ? (int)line.VolumePrimaryValue : null;
    }

    public string? ExchangeId { get; set; }
    public decimal Value { get; set; }
    public int? Volume { get; set; }
}
