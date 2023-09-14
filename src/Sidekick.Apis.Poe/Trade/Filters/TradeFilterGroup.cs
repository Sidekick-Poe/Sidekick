namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class TradeFilterGroup
    {
        public bool Disabled { get; set; }
        public TradeFilter Filters { get; set; } = new();
    }
}
