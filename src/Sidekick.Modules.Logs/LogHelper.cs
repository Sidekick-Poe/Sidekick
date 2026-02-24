using Serilog;
using Serilog.Events;
using Sidekick.Common;
namespace Sidekick.Modules.Logs;

public class LogHelper
{
    private static ILogger? logger;

    public static ILogger GetLogger(string fileName)
    {
        if (logger != null) return logger;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: SidekickPaths.GetDataFilePath(fileName),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 2,
                fileSizeLimitBytes: 5242880,
                rollOnFileSizeLimit: true)
            .WriteTo.Sink(LogSink.Instance)
            .CreateLogger();

        logger = Log.Logger;
        return logger;
    }
}
