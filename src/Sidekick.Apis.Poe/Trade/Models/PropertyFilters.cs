namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PropertyFilters
    {
        public string? ItemCategory { get; set; } = null;
        public bool TypeFilterEnabled { get; set; }
        public bool UseSpecificType { get; set; }
        public bool RarityFilterEnabled { get; set; }
        public List<PropertyFilter> Weapon { get; set; } = new();
        public List<PropertyFilter> Armour { get; set; } = new();
        public List<PropertyFilter> Map { get; set; } = new();
        public List<PropertyFilter> Misc { get; set; } = new();
    }
}
