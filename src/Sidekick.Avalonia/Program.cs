using Avalonia;
using Avalonia.Controls;
using Sidekick.Common.Platform;
using Velopack;

namespace Sidekick.Avalonia;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PlatformOrchestrator.Initialize();
        VelopackApp.Build().Run();

        // Hidden windows are closed to free their web view, so closing the last one must not quit the app living in the tray.
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
    }
}
