using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
namespace Sidekick.Modules.Logs;

public class LogSink : ILogEventSink
{
    public static LogSink Instance { get; } = new();

    private LogSink() { }

    private readonly ITextFormatter textFormatter = new MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

    public CappedQueue<string> Entries = new(1000);

    public void Emit(LogEvent logEvent)
    {
        _ = logEvent ?? throw new ArgumentNullException(nameof(logEvent));
        using var writer = new StringWriter();
        textFormatter.Format(logEvent, writer);

        var logMessage = writer.ToString();
        Entries.Enqueue(logMessage);
        LogEventEmitted?.Invoke(logMessage);
    }

    public event Action<string>? LogEventEmitted;
}
