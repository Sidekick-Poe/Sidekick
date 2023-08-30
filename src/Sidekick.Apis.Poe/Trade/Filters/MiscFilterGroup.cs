namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class MiscFilterGroup
    {
        public bool Disabled { get; set; }
        public MiscFilter Filters { get; set; } = new();
    }
}
