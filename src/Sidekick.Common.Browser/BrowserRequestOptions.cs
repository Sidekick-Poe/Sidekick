namespace Sidekick.Common.Browser;

public class BrowserRequestOptions
{
    public required Uri Uri { get; set; }

    private TaskCompletionSource<BrowserResult> TaskCompletion { get; } = new();

    internal Task<BrowserResult> Task => TaskCompletion.Task;

    public Func<BrowserCompletionOptions, bool> ShouldComplete { get; set; } = _ => true;

    public void SetResult(BrowserResult result)
    {
        if (TaskCompletion.Task.IsCompleted) return;

        TaskCompletion.SetResult(result);
    }

    public void SetFailed()
    {
        if (TaskCompletion.Task.IsCompleted) return;

        TaskCompletion.SetResult(new BrowserResult());
    }
}
