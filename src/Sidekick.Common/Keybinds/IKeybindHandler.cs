namespace Sidekick.Common.Keybinds
{
    /// <summary>
    /// Interface for keybind handlers
    /// </summary>
    public interface IKeybindHandler
    {
        List<string> GetKeybinds();

        /// <summary>
        /// When a keypress occurs, check if this keybind should be executed
        /// </summary>
        /// <returns>True if we need to execute this keybind</returns>
        bool IsValid();

        /// <summary>
        /// Executes when a valid keybind is detected
        /// </summary>
        /// <param name="keybind">The keybind that was pressed</param>
        /// <returns>A task</returns>
        Task Execute(string keybind);
    }
}
