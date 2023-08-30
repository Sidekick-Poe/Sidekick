namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class ArmorFilterGroup
    {
        public bool Disabled { get; set; }
        public ArmorFilter Filters { get; set; } = new();
    }
}
