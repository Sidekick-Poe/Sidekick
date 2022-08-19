using System.Collections.Generic;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class ModifierPatternMetadata
    {
        public string Id { get; set; }

        public ModifierCategory Category { get; set; }

        public bool IsOption { get; set; }

        public List<ModifierPattern> Patterns { get; set; } = new List<ModifierPattern>();
    }
}
