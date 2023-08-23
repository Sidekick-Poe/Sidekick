using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Sidekick.Common.Logging
{
    public class LogSink : ILogEventSink
    {
        private readonly ITextFormatter textFormatter = new MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null);

        public ConcurrentQueue<string> Events { get; } = new ConcurrentQueue<string>();

        public event Action<string> LogEventEmitted = null!;

        public void Emit(LogEvent logEvent)
        {
            _ = logEvent ?? throw new ArgumentNullException(nameof(logEvent));
            var writer = new StringWriter();
            textFormatter.Format(logEvent, writer);

            var logMessage = writer.ToString();
            LogEventEmitted?.Invoke(logMessage);
            Events.Enqueue(logMessage);
        }
    }
}
