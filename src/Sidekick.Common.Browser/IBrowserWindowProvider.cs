namespace Sidekick.Common.Browser;

public interface IBrowserWindowProvider
{
    event Action<BrowserRequest>? WindowOpened;

    Task<BrowserResult> OpenBrowserWindow(BrowserRequest options);

    Task SaveCookies(string clientName, BrowserResult result, CancellationToken cancellationToken);

    Task<string> GetCookieString(string clientName, CancellationToken cancellationToken);
}
