namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class NinjaCurrency
{
    public NinjaCurrency(ApiLine line, ApiOverviewResult result)
    {
        Id = line.Id;
        LastUpdated = result.LastUpdated;
        Sparkline = line.Sparkline;

        if (result.Core != null)
        {
            Trades = result.Core.Items.Select(x => new NinjaCurrencyTrade(line, result.Core, x.Id)).ToList();
        }
    }

    public string? Id { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public List<NinjaCurrencyTrade> Trades { get; set; } = [];
    public ApiSparkline? Sparkline { get; set; }
    public required Uri? DetailsUrl { get; set; }
}
