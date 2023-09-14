namespace Sidekick.Apis.Poe.Trade.Requests
{
    internal class QueryRequest
    {
        public Query Query { get; set; } = new();
        public Dictionary<string, SortType> Sort { get; set; } = new() { { "price", SortType.Asc } };
    }
}
