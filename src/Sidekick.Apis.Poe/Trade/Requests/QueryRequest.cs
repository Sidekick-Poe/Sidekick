namespace Sidekick.Apis.Poe.Trade.Requests
{
    internal class QueryRequest
    {
        public required Query Query { get; set; }

        public Dictionary<string, string> Sort { get; } = new() { { "price", "asc" } };
    }
}
