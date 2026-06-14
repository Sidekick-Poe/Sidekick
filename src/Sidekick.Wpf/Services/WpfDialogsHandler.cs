using System.Windows;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Dialogs;

namespace Sidekick.Wpf.Services;

public class WpfDialogsHandler : IDisposable
{
    private readonly IServiceProvider serviceProvider;
    private readonly BrowserDialogProvider browserDialogProvider;
    private readonly TransparentDialogProvider transparentDialogProvider;
    private readonly DialogProvider dialogProvider;
    private readonly ILogger<WpfDialogsHandler> logger;

    public WpfDialogsHandler(
        IServiceProvider serviceProvider,
        BrowserDialogProvider browserDialogProvider,
        TransparentDialogProvider transparentDialogProvider,
        DialogProvider dialogProvider,
        ILogger<WpfDialogsHandler> logger)
    {
        this.serviceProvider = serviceProvider;
        this.browserDialogProvider = browserDialogProvider;
        this.transparentDialogProvider = transparentDialogProvider;
        this.dialogProvider = dialogProvider;
        this.logger = logger;
        browserDialogProvider.Opened += BrowserOpened;
        dialogProvider.Opened += DialogOpened;
        transparentDialogProvider.Opened += TransparentOpened;
    }

    private void BrowserOpened(BrowserDialogProvider.OpenedArgs args)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var window = new BrowserWindow(logger, args);
            window.Show();
        });
    }

    private void DialogOpened(DialogProvider.OpenedArgs args)
    {
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                var window = new DialogWindow(serviceProvider, args.Type, args.Message);
                window.Show();
                var result = await window.Task;
                args.TaskCompletion.TrySetResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[WpfDialogsHandler] Error opening dialog.");
                args.TaskCompletion.TrySetResult(DialogProvider.Result.Closed);
            }
        });
    }

    private void TransparentOpened(TransparentDialogProvider.OpenedArgs args)
    {
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                logger.LogInformation("[WpfDialogsHandler] Opening transparent focus window.");

                var window = new TransparentFocusWindow();
                window.Show();

                await window.FocusTakenTask;
                args.TaskCompletion.TrySetResult();

                logger.LogInformation("[WpfDialogsHandler] Transparent focus window opened and focused.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[WpfDialogsHandler] Error opening transparent focus window.");
                args.TaskCompletion.TrySetException(ex);
            }
        });
    }

    public void Dispose()
    {
        browserDialogProvider.Opened -= BrowserOpened;
        dialogProvider.Opened -= DialogOpened;
        transparentDialogProvider.Opened -= TransparentOpened;
    }
}
