namespace Sidekick.Common.Browser;

public interface IBrowserWindowProvider
{
    event Action<BrowserRequest>? WindowOpened;

    Task<BrowserResult> OpenBrowserWindow(BrowserRequest options, CancellationToken cancellationToken = default);
}
