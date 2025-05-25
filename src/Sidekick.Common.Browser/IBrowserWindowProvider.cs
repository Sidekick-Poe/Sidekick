namespace Sidekick.Common.Browser;

public interface IBrowserWindowProvider
{
    event Action<BrowserRequestOptions>? WindowOpened;

    Task<BrowserResult> OpenBrowserWindow(BrowserRequestOptions options, CancellationToken cancellationToken = default);
}
