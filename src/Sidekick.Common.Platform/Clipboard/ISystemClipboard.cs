namespace Sidekick.Common.Platform.Clipboard;

/// <summary>
/// Reads and writes the text in the system clipboard, with the tool that fits the platform.
/// </summary>
public interface ISystemClipboard
{
    /// <summary>
    /// Gets the text in the clipboard, or null when it holds no text.
    /// </summary>
    Task<string?> GetText();

    /// <summary>
    /// Puts the text in the clipboard.
    /// </summary>
    Task SetText(string? text);
}
