using Avalonia;
using Sidekick.Common.Dialogs;

namespace Sidekick.Avalonia.Services;

public sealed class AvaloniaBrowserDialogHandler : IDisposable
{
    private readonly BrowserDialogProvider browserDialogProvider;
    private readonly ILogger<AvaloniaBrowserDialogHandler> logger;

    private BrowserWindow? browserWindow;

    public AvaloniaBrowserDialogHandler(
        BrowserDialogProvider browserDialogProvider,
        ILogger<AvaloniaBrowserDialogHandler> logger)
    {
        this.browserDialogProvider = browserDialogProvider;
        this.logger = logger;

        browserDialogProvider.Opened += BrowserDialogProvider_Opened;
    }

    private void BrowserDialogProvider_Opened(BrowserDialogProvider.OpenedArgs args)
    {
        var application = Application.Current;

        if (application is null)
        {
            args.TaskCompletion.TrySetResult(new BrowserDialogProvider.Result(
                Uri: args.Uri,
                Success: false,
                UserAgent: null,
                Cookies: new Dictionary<string, string>(),
                JsonContent: null));

            return;
        }

        application.Dispatcher.InvokeAsync(() =>
        {
            try
            {
                browserWindow?.Close();

                browserWindow = new BrowserWindow(logger, args);
                browserWindow.Closed += BrowserWindow_Closed;
                browserWindow.Show();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[AvaloniaBrowserDialogHandler] Error opening browser window.");

                args.TaskCompletion.TrySetResult(new BrowserDialogProvider.Result(
                    Uri: args.Uri,
                    Success: false,
                    UserAgent: null,
                    Cookies: new Dictionary<string, string>(),
                    JsonContent: null));
            }
        });
    }

    private void BrowserWindow_Closed(object? sender, EventArgs e)
    {
        if (sender is BrowserWindow window)
        {
            window.Closed -= BrowserWindow_Closed;
        }

        if (ReferenceEquals(browserWindow, sender))
        {
            browserWindow = null;
        }
    }

    public void Close()
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            browserWindow?.Close();
            browserWindow = null;
        });
    }

    public void Dispose()
    {
        browserDialogProvider.Opened -= BrowserDialogProvider_Opened;

        Close();
    }
}