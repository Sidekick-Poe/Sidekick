namespace Sidekick.Common.Game.GameLogs;

public interface IGameLogProvider
{
    /// <summary>
    ///     Gets the character name of the last whisper message that the player has received inside Path of Exile
    /// </summary>
    string? GetLatestWhisper();
}
