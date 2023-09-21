using System.Collections.Generic;
using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Modules.Trade.Models
{
    public class FiltersModel
    {
        public PropertyFilters PropertyFilters { get; set; }

        public List<ModifierFilter> ModifierFilters { get; set; }

        public List<PseudoModifierFilter> PseudoFilters { get; set; }
    }
}
