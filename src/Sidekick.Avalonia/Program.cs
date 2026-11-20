using Avalonia;
using Avalonia.Controls;
using Sidekick.Avalonia.Utilities;
using Sidekick.Common.Platform;
using Velopack;

namespace Sidekick.Avalonia;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (OperatingSystem.IsLinux())
        {
            // WebKitGTK's DMA-BUF renderer leaves every window blank on some drivers (NVIDIA among them).
            // A value the user set themselves is kept.
            LinuxNativeMethods.setenv("WEBKIT_DISABLE_DMABUF_RENDERER", "1", 0);

            WaylandSession.Initialize();
        }

        VelopackApp.Build().Run();

        // Hidden windows are closed to free their web view, so closing the last one must not quit the app living in the tray.
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
    }
}
