using Avalonia;
using Sidekick.Avalonia.Utilities;
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
        }

        VelopackApp.Build().Run();
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }
}
