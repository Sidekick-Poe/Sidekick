using Serilog.Events;

namespace Sidekick.Modules.Logs;

public class LogMonitor : IDisposable
{
    public int Warnings { get; private set; }
    public int Errors { get; private set; }

    public LogMonitor()
    {
        Warnings = 0;
        Errors = 0;
        LogSink.Instance.LogEventEmitted += OnLogEventEmitted;
    }

    private void OnLogEventEmitted(LogEventLevel level, string message)
    {
        if (level == LogEventLevel.Warning)
        {
            Warnings++;
        }
        else if (level >= LogEventLevel.Error)
        {
            Errors++;
        }
    }

    public void Dispose()
    {
        LogSink.Instance.LogEventEmitted -= OnLogEventEmitted;
    }
}
