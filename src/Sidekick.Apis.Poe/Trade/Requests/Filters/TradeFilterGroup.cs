namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class TradeFilterGroup
    {
        public bool Disabled { get; set; }
        public TradeFilter Filters { get; set; } = new();
    }
}
