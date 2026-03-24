namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class NinjaExchangeOverview
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public NinjaExchangeCore? Core { get; set; }

    public List<NinjaExchangeItem> Items { get; set; } = [];

    public List<NinjaExchangeLine> Lines { get; set; } = [];
}