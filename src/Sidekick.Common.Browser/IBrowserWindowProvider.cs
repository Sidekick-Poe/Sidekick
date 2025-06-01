namespace Sidekick.Common.Browser;

/// <summary>
/// Provides an interface for managing browser window interactions, such as opening
/// browser windows, handling cookie management, and notifying when a browser window
/// is opened.
/// </summary>
public interface IBrowserWindowProvider
{
    event Action<BrowserRequest>? WindowOpened;

    Task<BrowserResult> OpenBrowserWindow(BrowserRequest options);

    Task SaveCookies(string clientName, BrowserResult result, CancellationToken cancellationToken);

    Task<string> GetCookieString(string clientName, CancellationToken cancellationToken);
}
