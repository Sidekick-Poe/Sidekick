using Microsoft.Extensions.Logging;
using Sidekick.Common.Dialogs.Browsers;
namespace Sidekick.Common.Dialogs.Transparent;

/// <summary>
/// Creates a transparent window, to take away the focus from the application.
/// </summary>
public class TransparentWindowProvider
(
    ILogger<TransparentWindowProvider> logger
)
{
    public event Action<TransparentWindowOptions>? WindowOpened;

    public event Action<TransparentWindowOptions>? WindowClosed;

    public async Task Open()
    {
        logger.LogInformation("[TransparentWindowProvider] Opening transparent window");
        var options = new TransparentWindowOptions();
        WindowOpened?.Invoke(options);
        await options.TaskCompletion.Task;
    }

    public async Task Close(string clientName, BrowserResult result, CancellationToken cancellationToken)
    {
        logger.LogInformation("[TransparentWindowProvider] Closing transparent window");
        var options = new TransparentWindowOptions();
        WindowClosed?.Invoke(options);
        await options.TaskCompletion.Task;
    }
}
