namespace Sidekick.Apis.PoeNinja.Currency.Models;

public class NinjaCurrency
{
    public NinjaCurrency(ApiLine line, ApiCore core, string? exchangeId)
    {
        decimal rate = 1;
        if (exchangeId != null) core.Rates.TryGetValue(exchangeId, out rate);

        Id = line.Id;
        Value = line.PrimaryValue * rate;
        Volume = line.VolumePrimaryValue * rate;
        Sparkline = line.Sparkline;
        if (Sparkline != null) Sparkline.Data = Sparkline.Data.Select(x => x * rate).ToList();
    }

    public string? Id { get; set; }
    public decimal Value { get; set; }
    public decimal Volume { get; set; }
    public ApiSparkline? Sparkline { get; set; }
}
