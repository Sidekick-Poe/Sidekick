using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Common.Game.Items
{
    public class Item
    {
        public Item(
            ItemMetadata metadata,
            OriginalItem original,
            Properties properties,
            Influences influences,
            List<Socket> sockets,
            List<ModifierLine> modifierLines,
            List<Modifier> pseudoModifiers)
        {
            Metadata = metadata;
            Original = original;
            Properties = properties;
            Influences = influences;
            Sockets = sockets;
            ModifierLines = modifierLines;
            PseudoModifiers = pseudoModifiers;
        }

        public ItemMetadata Metadata { get; }

        public OriginalItem Original { get; }

        public Properties Properties { get; }

        public Influences Influences { get; }

        public List<Socket> Sockets { get; }

        public List<ModifierLine> ModifierLines { get; set; }

        public List<Modifier> PseudoModifiers { get; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            if (Metadata == null)
            {
                return "null";
            }

            if (!string.IsNullOrEmpty(Metadata.Name) && !string.IsNullOrEmpty(Metadata.Type) && Metadata.Name != Metadata.Type)
            {
                return $"{Metadata.Name} - {Metadata.Type}";
            }

            if (!string.IsNullOrEmpty(Metadata.Type))
            {
                return Metadata.Type;
            }

            return Metadata.Name;
        }

        public int GetMaximumNumberOfLinks() => Sockets.GroupBy(x => x.Group).Max(x => x.Count());
    }
}
