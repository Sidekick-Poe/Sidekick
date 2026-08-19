namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class ApiExchangeCore
{
    public string? Primary { get; set; }

    public string? Secondary { get; set; }

    public Dictionary<string, decimal> Rates { get; set; } = [];

    public List<ApiExchangeItem> Items { get; set; } = [];
}