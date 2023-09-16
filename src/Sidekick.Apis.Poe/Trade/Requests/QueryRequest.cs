namespace Sidekick.Apis.Poe.Trade.Requests
{
    internal class QueryRequest
    {
        public Query Query { get; set; } = new();

        public Dictionary<string, string> Sort { get; } = new() { { "price", "asc" } };
    }
}
