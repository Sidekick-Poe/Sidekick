namespace Sidekick.Apis.Poe.Trade.Requests
{
    public class BulkQueryRequest
    {
        public BulkQueryRequest()
        {
            Exchange.Status.Option = StatusType.Online;
        }

        public Exchange Exchange { get; } = new();
        public Dictionary<string, SortType> Sort { get; } = new() { { "price", SortType.Asc } };
    }
}
