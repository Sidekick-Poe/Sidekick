namespace Sidekick.Common.Browser;

public interface IBrowserProvider
{
    /// <summary>
    ///     Opens the specified Uri in a web browser
    /// </summary>
    /// <param name="uri">The uri to open in a browser</param>
    void OpenUri(Uri uri);

    /// <summary>
    ///     Opens the sidekick website in the user's browser.
    /// </summary>
    void OpenSidekickWebsite();

    /// <summary>
    ///    Opens the Sidekick GitHub repository in the user's browser.
    /// </summary>
    void OpenGitHubRepository();
}
