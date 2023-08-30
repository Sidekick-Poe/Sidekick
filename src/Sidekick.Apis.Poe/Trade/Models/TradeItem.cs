using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class TradeItem : Item
    {
        public TradeItem(
            ItemMetadata metadata,
            OriginalItem original,
            Properties properties,
            Influences influences,
            List<Socket> sockets,
            List<ModifierLine> modifierLines,
            List<Modifier> pseudoModifiers)
            : base(metadata, original, properties, influences, sockets, modifierLines, pseudoModifiers)
        {
        }

        public string? Id { get; set; }
        public TradePrice? Price { get; set; }

        public string? Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<LineContent> PropertyContents { get; set; } = new();
        public List<LineContent> AdditionalPropertyContents { get; set; } = new();
        public List<LineContent> RequirementContents { get; set; } = new();
    }
}
