namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class MapFilterGroup
    {
        public bool Disabled { get; set; }
        public MapFilter Filters { get; set; } = new();
    }
}
