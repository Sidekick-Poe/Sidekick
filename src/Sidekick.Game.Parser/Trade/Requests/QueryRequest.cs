namespace Sidekick.Game.Parser.Trade.Requests;

public class QueryRequest
{
    public required Query Query { get; set; }

    public Dictionary<string, string> Sort { get; } = new() { { "price", "asc" } };
}
