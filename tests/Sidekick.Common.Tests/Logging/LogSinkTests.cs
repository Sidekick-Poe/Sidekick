using Serilog.Events;
using Sidekick.Modules.Logs;
using Xunit;

namespace Sidekick.Common.Tests.Logging;

public class LogSinkTests
{
    [Fact]
    public void LogService_StoresLast1000Logs()
    {
        for (int i = 0; i < 1100; i++)
        {
            LogSink.Instance.Emit(new LogEvent(
                      DateTimeOffset.Now,
                      LogEventLevel.Information,
                      null,
                      new Serilog.Parsing.MessageTemplateParser().Parse("Test {i}"),
                      new[] { new LogEventProperty("i", new ScalarValue(i)) }
                      ));
        }

        Assert.Equal(1000, LogSink.Instance.Entries.Count);

        var list = LogSink.Instance.Entries.ToList();
        Assert.Contains("Test 100", list.First()); // Test 0 to 99 should be gone
        Assert.Contains("Test 1099", list.Last());
    }

    [Fact]
    public void LogService_EmitsEvent()
    {
        string? emittedMessage = null;
        LogSink.Instance.LogEventEmitted += (level, msg) => emittedMessage = msg;

        LogSink.Instance.Emit(new LogEvent(
                              DateTimeOffset.Now,
                              LogEventLevel.Information,
                              null,
                              new Serilog.Parsing.MessageTemplateParser().Parse("Test Message"),
                              Enumerable.Empty<LogEventProperty>()
                              ));

        Assert.NotNull(emittedMessage);
        Assert.Contains("Test Message", emittedMessage);
    }

    [Fact]
    public void LogMonitor_TracksWarningsAndErrors()
    {
        var monitor = new LogMonitor();
        using (monitor.Monitor())
        {
            EmitLog(LogEventLevel.Information, "Info");
            EmitLog(LogEventLevel.Warning, "Warning 1");
            EmitLog(LogEventLevel.Warning, "Warning 2");
            EmitLog(LogEventLevel.Error, "Error 1");
            EmitLog(LogEventLevel.Fatal, "Fatal 1");
        }

        Assert.Equal(2, monitor.Warnings);
        Assert.Equal(2, monitor.Errors);

        // Should not track after disposal
        EmitLog(LogEventLevel.Warning, "Warning 3");
        EmitLog(LogEventLevel.Error, "Error 2");

        Assert.Equal(2, monitor.Warnings);
        Assert.Equal(2, monitor.Errors);
    }

    private void EmitLog(LogEventLevel level, string message)
    {
        LogSink.Instance.Emit(new LogEvent(
            DateTimeOffset.Now,
            level,
            null,
            new Serilog.Parsing.MessageTemplateParser().Parse(message),
            Enumerable.Empty<LogEventProperty>()
        ));
    }
}