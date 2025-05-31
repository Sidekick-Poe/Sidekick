namespace Sidekick.Common.Browser;

public class BrowserRequest
{
    public required Uri Uri { get; set; }

    internal TaskCompletionSource<BrowserResult> TaskCompletion { get; } = new();

    public Func<BrowserCompletionOptions, bool> ShouldComplete { get; set; } = _ => false;

    public Task<BrowserResult> Task => TaskCompletion.Task;

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
