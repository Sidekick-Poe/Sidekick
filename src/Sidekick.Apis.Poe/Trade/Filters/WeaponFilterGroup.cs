namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class WeaponFilterGroup
    {
        public bool Disabled { get; set; }
        public WeaponFilter Filters { get; set; } = new();
    }
}
