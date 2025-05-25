using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Browser;

public class BrowserWindowProvider
(
    ILogger<BrowserWindowProvider> logger
) : IBrowserWindowProvider
{
    public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

    public event Action<BrowserRequest>? WindowOpened;

    public async Task<BrowserResult> OpenBrowserWindow(BrowserRequest options, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[BrowserWindowProvider] Opening browser window.");
        WindowOpened?.Invoke(options);
        return await options.TaskCompletion.Task;
    }
}
