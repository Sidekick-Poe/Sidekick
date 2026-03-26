using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Data.Items;
namespace Sidekick.Apis.PoeNinja.Exchange;

public class NinjaCurrency
{
    public NinjaCurrency(NinjaExchangeLine line, NinjaExchangeOverview result)
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
    public NinjaSparkline? Sparkline { get; set; }
    public required Uri? DetailsUrl { get; set; }
}
