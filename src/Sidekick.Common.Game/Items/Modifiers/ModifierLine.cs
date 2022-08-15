using System.Collections.Generic;

namespace Sidekick.Common.Game.Items.Modifiers
{
    /// <summary>
    /// Represents a line of text on an item. With the API being the way it is, each line of text can be represented by one or more api modifiers.
    /// </summary>
    public class ModifierLine
    {
        /// <summary>
        /// The original line of text as it is in the game.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The list of modifiers that matches the game text.
        /// </summary>
        public List<Modifier> Modifiers { get; set; } = new();
    }
}
