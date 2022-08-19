using System.Collections.Generic;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Common.Game.Items
{
    public class Item
    {
        public ItemMetadata Metadata { get; set; } = new();

        public OriginalItem Original { get; set; } = new();

        public Properties Properties { get; set; } = new();

        public Influences Influences { get; set; } = new();

        public List<Socket> Sockets { get; set; } = new();

        public List<ModifierLine> ModifierLines { get; set; } = new();

        public List<Modifier> PseudoModifiers { get; set; } = new();
    }
}
