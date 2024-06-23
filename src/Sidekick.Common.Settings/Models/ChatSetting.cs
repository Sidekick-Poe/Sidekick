namespace Sidekick.Common.Settings.Models;

/// <summary>
///     Send a message in the chat
/// </summary>
public class ChatSetting
{
    /// <summary>
    ///     Send a message in the chat. Wildcards are supported.
    /// </summary>
    public ChatSetting()
    {
    }

    /// <summary>
    ///     Send a message in the chat. Wildcards are supported.
    /// </summary>
    /// <param name="key">The keybind to send the command</param>
    /// <param name="command">The message to write in the chat</param>
    /// <param name="submit">Indicated if the message should be submitted after it is written</param>
    public ChatSetting(
        string key,
        string command,
        bool submit)
    {
        Key = key;
        Command = command;
        Submit = submit;
    }

    /// <summary>
    ///     The keybind to send the command
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    ///     The message to write in the chat
    /// </summary>
    public string? Command { get; set; }

    /// <summary>
    ///     Indicated if the message should be submitted after it is written
    /// </summary>
    public bool Submit { get; set; }
}
