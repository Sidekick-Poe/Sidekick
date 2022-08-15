using System.Collections.Generic;

namespace Sidekick.Common.Game.Items.Modifiers
{
    /// <summary>
    /// Represents a line of text on an item. With the API being the way it is, each line of text can be represented by one or more api modifiers.
    /// </summary>
    public class ModifierLine
    {
        /// <summary>
        /// Gets or sets the original line of text as it is in the game.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the modifier associated with this line.
        /// </summary>
        public Modifier Modifier { get; set; }

        /// <summary>
        /// Gets or sets the list of modifiers that also matches the game text, or similar phrased modifiers if fuzzy search was used.
        /// </summary>
        public List<Modifier> Alternates { get; set; } = new();
    }
}
