namespace Sidekick.Common.Browser;

public interface IBrowserProvider
{
    /// <summary>
    /// Opens the specified Uri in a web browser
    /// </summary>
    /// <param name="uri">The uri to open in a browser</param>
    void OpenUri(Uri uri);

    /// <summary>
    /// The URL to the Sidekick website.
    /// </summary>
    Uri SidekickWebsite { get; }

    /// <summary>
    /// The URL to the Sidekick GitHub repository.
    /// </summary>
    Uri GitHubRepository { get; }

    /// <summary>
    /// The URL to the Sidekick Discord server.
    /// </summary>
    Uri DiscordServer { get; }
}
