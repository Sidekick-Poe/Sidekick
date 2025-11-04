namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class ApiCore
{
    public string? Primary { get; set; }

    public string? Secondary { get; set; }

    public Dictionary<string, decimal> Rates { get; set; } = [];

    public List<ApiItem> Items { get; set; } = [];
}