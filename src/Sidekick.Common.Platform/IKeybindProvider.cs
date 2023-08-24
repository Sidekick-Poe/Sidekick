using Sidekick.Common.Initialization;
using Sidekick.Common.Keybinds;

namespace Sidekick.Common.Platform
{
    /// <summary>
    /// Service providing keybind functions
    /// </summary>
    public interface IKeybindProvider : IInitializableService
    {
        /// <summary>
        /// A list of keybind handlers currently registered to the application
        /// </summary>
        Dictionary<string, IKeybindHandler> KeybindHandlers { get; }
    }
}
