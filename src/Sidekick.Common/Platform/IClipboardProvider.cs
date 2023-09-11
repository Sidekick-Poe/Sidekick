namespace Sidekick.Common.Platform
{
    /// <summary>
    /// Provides a provider to access clipboard features on the operating system
    /// </summary>
    public interface IClipboardProvider
    {
        /// <summary>
        /// Sends a copy command (Ctrl+C). If the setting to preserve the clipboard is turned on,
        /// the clipboard is restored to the initial value. The text that was copied is returned by
        /// this task.
        /// </summary>
        /// <returns>The text that was copied</returns>
        Task<string?> Copy();

        /// <summary>
        /// Sends an advanced copy command (Ctrl+Alt+C). If the setting to preserve the clipboard is turned on,
        /// the clipboard is restored to the initial value. The text that was copied is returned by
        /// this task.
        /// </summary>
        /// <returns>The text that was copied</returns>
        Task<string?> CopyAdvanced();

        /// <summary>
        /// Gets the text value of what is currently in the clipboard.
        /// </summary>
        /// <returns>The text that is in the clipboard.</returns>
        Task<string?> GetText();

        /// <summary>
        /// Sets the value of the clipboard to the text passed as parameter.
        /// </summary>
        /// <param name="text">The text to set the clipboard to.</param>
        /// <returns>The task</returns>
        Task SetText(string? text);
    }
}
