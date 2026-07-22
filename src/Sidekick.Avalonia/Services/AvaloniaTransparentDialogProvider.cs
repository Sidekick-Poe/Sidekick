using Avalonia;
using Sidekick.Common.Dialogs;

namespace Sidekick.Avalonia.Services;

public sealed class AvaloniaTransparentDialogProvider : IDisposable
{
    private readonly TransparentDialogProvider dialogProvider;
    private readonly ILogger<AvaloniaTransparentDialogProvider> logger;

    public AvaloniaTransparentDialogProvider(
        TransparentDialogProvider dialogProvider,
        ILogger<AvaloniaTransparentDialogProvider> logger)
    {
        this.dialogProvider = dialogProvider;
        this.logger = logger;

        dialogProvider.Opened += DialogOpened;
    }

    private void DialogOpened(TransparentDialogProvider.OpenedArgs args)
    {
        var application = Application.Current;

        if (application is null)
        {
            args.TaskCompletion.TrySetResult();
            return;
        }

        application.Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                var window = new TransparentWindow();
                window.Show();

                await window.FocusTakenTask;
                args.TaskCompletion.TrySetResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[AvaloniaTransparentDialogProvider] Error opening transparent window.");
                args.TaskCompletion.TrySetException(ex);
            }
        });
    }

    public void Dispose()
    {
        dialogProvider.Opened -= DialogOpened;
    }
}
