using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

public sealed class ServerSynchronizedRateLimiter : RateLimiter
{
    private readonly object _lock = new();
    private int _currentHitCount;
    private int _maxHitCount;
    private TimeSpan _timePeriod;
    private DateTimeOffset _windowEnd;
    private System.Threading.Timer? _timer;

    public event Action? OnChange;

    public ServerSynchronizedRateLimiter(int maxHitCount, TimeSpan timePeriod)
    {
        _maxHitCount = maxHitCount;
        _timePeriod = timePeriod;
        _windowEnd = DateTimeOffset.UtcNow + _timePeriod;
        ScheduleTimerLocked();
    }

    public int CurrentHitCount
    {
        get { lock (_lock) return _currentHitCount; }
    }

    public int MaxHitCount
    {
        get { lock (_lock) return _maxHitCount; }
    }

    /// <summary>
    /// Called when you receive fresh headers from the server.
    /// </summary>
    public void UpdateFromServer(int currentHitCount, DateTimeOffset now)
    {
        lock (_lock)
        {
            _currentHitCount = currentHitCount;

            // Start a new window from "now". If you know the remaining time instead,
            // you can set _windowEnd accordingly.
            _windowEnd = now + _timePeriod;
            ScheduleTimerLocked();
        }
    }

    private void RefreshWindow(DateTimeOffset now)
    {
        if (now >= _windowEnd)
        {
            _currentHitCount = 0;
            _windowEnd = now + _timePeriod;
            // notify after replenishment
            OnChangeSafe();
            // reschedule next tick
            ScheduleTimerLocked();
        }
    }

    public override TimeSpan? IdleDuration
    {
        get
        {
            lock (_lock)
            {
                RefreshWindow(DateTimeOffset.UtcNow);
                if (_currentHitCount < _maxHitCount) return TimeSpan.Zero;
                return _windowEnd - DateTimeOffset.UtcNow;
            }
        }
    }

    public override RateLimiterStatistics? GetStatistics() => null;

    protected override RateLimitLease AttemptAcquireCore(int permitCount)
    {
        lock (_lock)
        {
            RefreshWindow(DateTimeOffset.UtcNow);

            if (permitCount <= 0)
            {
                return new ServerSynchronizedRateLimitLease(true, 0, null);
            }

            if (_currentHitCount + permitCount <= _maxHitCount)
            {
                _currentHitCount += permitCount;
                return new ServerSynchronizedRateLimitLease(true, permitCount, null);
            }

            var retryAfter = _windowEnd - DateTimeOffset.UtcNow;
            if (retryAfter < TimeSpan.Zero) retryAfter = TimeSpan.Zero;
            return new ServerSynchronizedRateLimitLease(false, 0, retryAfter);
        }
    }

    protected override ValueTask<RateLimitLease> AcquireAsyncCore(
        int permitCount,
        CancellationToken cancellationToken)
    {
        // Simple implementation: if we can't acquire now, wait until the end of the
        // current window (or until cancellation), then try again once.
        lock (_lock)
        {
            var lease = (ServerSynchronizedRateLimitLease)AttemptAcquireCore(permitCount);
            if (lease.IsAcquired || lease.RetryAfter == null)
            {
                return new ValueTask<RateLimitLease>(lease);
            }

            var delay = lease.RetryAfter.Value;
            return WaitAndAcquireAsync(permitCount, delay, cancellationToken);
        }
    }

    private async ValueTask<RateLimitLease> WaitAndAcquireAsync(
        int permitCount,
        TimeSpan delay,
        CancellationToken cancellationToken)
    {
        if (delay > TimeSpan.Zero)
        {
            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return new ServerSynchronizedRateLimitLease(false, 0, null);
            }
        }

        lock (_lock)
        {
            return AttemptAcquireCore(permitCount);
        }
    }

    protected override void Dispose(bool disposing)
    {
        // Clean up timer
        var timer = _timer;
        if (timer != null)
        {
            _timer = null;
            timer.Dispose();
        }
    }

    private void ScheduleTimerLocked()
    {
        // assumes lock is held by caller (constructor is single-threaded too)
        var due = _windowEnd - DateTimeOffset.UtcNow;
        if (due < TimeSpan.Zero) due = TimeSpan.Zero;

        if (_timer == null)
        {
            _timer = new System.Threading.Timer(static s =>
            {
                var self = (ServerSynchronizedRateLimiter)s!;
                lock (self._lock)
                {
                    self.RefreshWindow(DateTimeOffset.UtcNow);
                }
            }, this, due, System.Threading.Timeout.InfiniteTimeSpan);
        }
        else
        {
            _timer.Change(due, System.Threading.Timeout.InfiniteTimeSpan);
        }
    }

    private void OnChangeSafe()
    {
        try
        {
            OnChange?.Invoke();
        }
        catch
        {
            // ignore listener exceptions
        }
    }
}
