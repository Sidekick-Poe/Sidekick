namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class StatFilterGroup
    {
        public StatType Type { get; set; }
        public List<StatFilter> Filters { get; set; } = new();
    }
}
