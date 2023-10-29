namespace Sidekick.Common.Game.Items
{
    public class Item
    {
        public Item(
            ItemMetadata metadata,
            Header header,
            Properties properties,
            Influences influences,
            List<Socket> sockets,
            List<ModifierLine> modifierLines,
            List<PseudoModifier> pseudoModifiers,
            string text)
        {
            Metadata = metadata;
            Header = header;
            Properties = properties;
            Influences = influences;
            Sockets = sockets;
            ModifierLines = modifierLines;
            PseudoModifiers = pseudoModifiers;
            Text = text;
        }

        public ItemMetadata Metadata { get; init; }

        public Header Header { get; init; }

        public Properties Properties { get; init; }

        public Influences Influences { get; init; }

        public List<Socket> Sockets { get; init; }

        public List<ModifierLine> ModifierLines { get; set; }

        public List<PseudoModifier> PseudoModifiers { get; init; }

        public string Text { get; set; }

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

        public int GetMaximumNumberOfLinks() => Sockets.Any() ? Sockets.GroupBy(x => x.Group).Max(x => x.Count()) : 0;
    }
}
