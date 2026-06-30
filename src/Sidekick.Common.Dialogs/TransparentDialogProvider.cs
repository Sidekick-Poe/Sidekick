using Microsoft.Extensions.Logging;
namespace Sidekick.Common.Dialogs;

/// <summary>
/// Creates a transparent window, to take away the focus from the application.
/// </summary>
public class TransparentDialogProvider
(
    ILogger<TransparentDialogProvider> logger
)
{
    public record OpenedArgs(TaskCompletionSource TaskCompletion);

    public event Action<OpenedArgs>? Opened;

    public async Task Open()
    {
        logger.LogInformation("[TransparentWindowProvider] Opening transparent window");
        var options = new OpenedArgs(new TaskCompletionSource());
        Opened?.Invoke(options);
        await options.TaskCompletion.Task;
    }
}
