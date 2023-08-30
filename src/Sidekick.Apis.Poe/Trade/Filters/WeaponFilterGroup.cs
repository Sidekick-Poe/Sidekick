namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class WeaponFilterGroup
    {
        public bool Disabled { get; set; }
        public WeaponFilter Filters { get; set; } = new();
    }
}
