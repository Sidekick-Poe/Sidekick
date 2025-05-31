using System.Windows;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Browser;
using Sidekick.Wpf.Browser;

namespace Sidekick.Wpf.Services;

public class WpfBrowserWindowProvider : IDisposable
{
    private readonly IBrowserWindowProvider browserWindowProvider;
    private readonly ILogger<WpfBrowserWindowProvider> logger;

    public WpfBrowserWindowProvider(IBrowserWindowProvider browserWindowProvider, ILogger<WpfBrowserWindowProvider> logger)
    {
        this.browserWindowProvider = browserWindowProvider;
        this.logger = logger;
        browserWindowProvider.WindowOpened += BrowserWindowProviderOnWindowOpened;
    }

    private void BrowserWindowProviderOnWindowOpened(BrowserRequest options)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var window = new BrowserWindow(logger, options);
            window.Show();
        });
    }

    public void Dispose()
    {
        browserWindowProvider.WindowOpened -= BrowserWindowProviderOnWindowOpened;
    }
}
