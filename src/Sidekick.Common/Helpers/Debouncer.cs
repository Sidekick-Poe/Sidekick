namespace Sidekick.Common.Helpers;

public class Debouncer : IDisposable
{
    private CancellationTokenSource? cts;
    private readonly Lock @lock = new();
    private bool disposed;

    /// <summary>
    /// Debounces an action. If called multiple times, the previous call is cancelled
    /// and the timer resets.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="delayMilliseconds">The wait time in milliseconds.</param>
    public void Debounce(Action action, int delayMilliseconds)
    {
        lock (@lock)
        {
            if (disposed) return;

            // Cancel the previous pending execution
            cts?.Cancel();
            cts?.Dispose();

            cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Delay(delayMilliseconds, token)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully && !token.IsCancellationRequested)
                    {
                        action();
                    }
                }, TaskScheduler.Default); // Ensures it runs on a thread pool thread
        }
    }

    public void Dispose()
    {
        lock (@lock)
        {
            if (disposed) return;
            disposed = true;
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}