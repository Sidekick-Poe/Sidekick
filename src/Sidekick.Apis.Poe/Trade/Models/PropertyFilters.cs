namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PropertyFilters
    {
        public bool BaseTypeFilterApplied { get; set; } = true;
        public bool ClassFilterApplied { get; set; }
        public bool RarityFilterApplied { get; set; }
        public List<PropertyFilter> Weapon { get; set; } = new();
        public List<PropertyFilter> Armour { get; set; } = new();
        public List<PropertyFilter> Map { get; set; } = new();
        public List<PropertyFilter> Misc { get; set; } = new();
    }
}
