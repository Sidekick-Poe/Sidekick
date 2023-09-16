using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class ModifierPatternMetadata2
    {
        public ModifierPatternMetadata2(
            ModifierCategory category,
            string id,
            bool isOption)
        {
            Category = category;
            Id = id;
            IsOption = isOption;
        }

        public string Id { get; }

        public ModifierCategory Category { get; }

        public bool IsOption { get; }

        public List<ModifierPattern> Patterns { get; } = new();
    }
}
