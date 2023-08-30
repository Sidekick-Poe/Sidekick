namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class TradeFilterGroup
    {
        public bool Disabled { get; set; }
        public TradeFilter Filters { get; set; } = new();
    }
}
