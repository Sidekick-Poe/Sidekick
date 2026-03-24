namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class NinjaExchangeCore
{
    public string? Primary { get; set; }

    public string? Secondary { get; set; }

    public Dictionary<string, decimal> Rates { get; set; } = [];

    public List<NinjaExchangeItem> Items { get; set; } = [];
}