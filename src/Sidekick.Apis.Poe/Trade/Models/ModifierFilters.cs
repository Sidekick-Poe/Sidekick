using System.Collections.Generic;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class ModifierFilters
    {
        public List<ModifierFilter> Implicit { get; set; } = new();
        public List<ModifierFilter> Explicit { get; set; } = new();
        public List<ModifierFilter> Crafted { get; set; } = new();
        public List<ModifierFilter> Enchant { get; set; } = new();
        public List<ModifierFilter> Pseudo { get; set; } = new();
        public List<ModifierFilter> Fractured { get; set; } = new();
        public List<ModifierFilter> Scourge { get; set; } = new();
    }
}
