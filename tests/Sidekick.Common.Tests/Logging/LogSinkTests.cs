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
            LogSink.Instance.Emit(new Serilog.Events.LogEvent(
                      DateTimeOffset.Now,
                      Serilog.Events.LogEventLevel.Information,
                      null,
                      new Serilog.Events.MessageTemplate(new[] { new Serilog.Parsing.TextToken("Test " + i) }),
                      Enumerable.Empty<Serilog.Events.LogEventProperty>()
                      ));
        }

        Assert.Equal(1000, LogSink.Instance.Entries.Count);

        var list = LogSink.Instance.Entries.ToList().ToList();
        Assert.Contains("Test 100", list.First()); // Test 0 to 99 should be gone
        Assert.Contains("Test 1099", list.Last());
    }

    [Fact]
    public void LogService_EmitsEvent()
    {
        string? emittedMessage = null;
        LogSink.Instance.LogEventEmitted += (msg) => emittedMessage = msg;

        LogSink.Instance.Emit(new Serilog.Events.LogEvent(
                              DateTimeOffset.Now,
                              Serilog.Events.LogEventLevel.Information,
                              null,
                              new Serilog.Events.MessageTemplate(new[] { new Serilog.Parsing.TextToken("Test Message") }),
                              Enumerable.Empty<Serilog.Events.LogEventProperty>()
                              ));

        Assert.NotNull(emittedMessage);
        Assert.Contains("Test Message", emittedMessage);
    }
}