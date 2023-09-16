namespace Sidekick.Apis.Poe.Trade.Requests
{
    public class BulkQueryRequest
    {
        public string Engine { get; } = "new";

        public BulkQuery Query { get; } = new();

        public Dictionary<string, string> Sort { get; } = new() { { "have", "asc" } };
    }
}
