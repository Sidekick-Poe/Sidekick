using System.Collections.Generic;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class ModifierFilter
    {
        public ModifierLine Line { get; set; }

        public bool Enabled { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }
    }
}
