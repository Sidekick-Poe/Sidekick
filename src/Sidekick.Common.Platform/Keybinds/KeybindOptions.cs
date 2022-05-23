using System;
using System.Collections.Generic;

namespace Sidekick.Common.Platform.Keybinds
{
    /// <summary>
    /// Contains runtime options for the Sidekick project
    /// </summary>
    public class KeybindOptions
    {
        /// <summary>
        /// The list of keybinds handled by this application
        /// </summary>
        public List<Type> Keybinds { get; set; } = new();
    }
}
