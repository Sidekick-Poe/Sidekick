namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class TradeFilterGroup
    {
        public bool Disabled { get; set; }
        public TradeFilters Filters { get; set; } = new();
    }
}
