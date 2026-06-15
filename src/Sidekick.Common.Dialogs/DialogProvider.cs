namespace Sidekick.Common.Dialogs;

public class DialogProvider
{
    public enum Type
    {
        Confirmation,
        Ok,
    }

    public enum Result
    {
        Closed,
        Confirmed,
    }

    public record OpenedArgs(Type Type, string Message, TaskCompletionSource<Result> TaskCompletion);
    public event Action<OpenedArgs>? Opened;

    /// <summary>
    ///     Open a notification message with Yes/No buttons
    /// </summary>
    /// <param name="message">The message to show in the notification</param>
    /// <returns>True if the user confirmed the action.</returns>
    public async Task<bool> OpenConfirmationModal(string message)
    {
        var args = new OpenedArgs(Type.Confirmation, message, new TaskCompletionSource<Result>(TaskCreationOptions.RunContinuationsAsynchronously));

        if (Opened is null) throw new InvalidOperationException("No dialog handler is registered.");
        Opened?.Invoke(args);

        var result = await args.TaskCompletion.Task;
        return result == Result.Confirmed;
    }

    /// <summary>
    /// Open a notification message with an OK button.
    /// </summary>
    /// <param name="message">The message to display in the notification.</param>
    /// <returns>True if the user acknowledged the message.</returns>
    public async Task<bool> OpenOkModal(string message)
    {
        var args = new OpenedArgs(Type.Ok, message, new TaskCompletionSource<Result>(TaskCreationOptions.RunContinuationsAsynchronously));

        if (Opened is null) throw new InvalidOperationException("No dialog handler is registered.");
        Opened?.Invoke(args);

        await args.TaskCompletion.Task;
        return true;
    }
}
