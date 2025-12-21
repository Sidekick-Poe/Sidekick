namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests;

public class QueryRequest
{
    public required Query Query { get; set; }

    public Dictionary<string, string> Sort { get; } = new() { { "price", "asc" } };
}
