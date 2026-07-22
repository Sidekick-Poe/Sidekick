using Avalonia;
using Sidekick.Common.Dialogs;

namespace Sidekick.Avalonia.Services;

public sealed class AvaloniaDialogsHandler : IDisposable
{
    private readonly DialogProvider dialogProvider;
    private readonly ILogger<AvaloniaDialogsHandler> logger;

    public AvaloniaDialogsHandler(
        DialogProvider dialogProvider,
        ILogger<AvaloniaDialogsHandler> logger)
    {
        this.dialogProvider = dialogProvider;
        this.logger = logger;

        dialogProvider.Opened += DialogOpened;
    }

    private void DialogOpened(DialogProvider.OpenedArgs args)
    {
        var application = Application.Current;

        if (application is null)
        {
            args.TaskCompletion.TrySetResult(DialogProvider.Result.Closed);
            return;
        }

        application.Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                var window = new DialogWindow(args.Type, args.Message);
                window.Show();

                var result = await window.Task;
                args.TaskCompletion.TrySetResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[AvaloniaDialogsHandler] Error opening dialog.");
                args.TaskCompletion.TrySetResult(DialogProvider.Result.Closed);
            }
        });
    }

    public void Dispose()
    {
        dialogProvider.Opened -= DialogOpened;
    }
}
