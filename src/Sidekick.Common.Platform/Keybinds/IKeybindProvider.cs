using System.Collections.Generic;

namespace Sidekick.Common.Platform.Keybinds
{
    /// <summary>
    /// Service providing keybind functions
    /// </summary>
    public interface IKeybindProvider
    {
        /// <summary>
        /// A list of keybind handlers currently registered to the application
        /// </summary>
        Dictionary<string, IKeybindHandler> KeybindHandlers { get; }

        /// <summary>
        /// Initializes keybinds. If already initialized, this can be called again to refresh the
        /// keybinds, such as when settings change.
        /// </summary>
        void Initialize();
    }
}
