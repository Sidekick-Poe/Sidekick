using Avalonia;
using Velopack;

namespace Sidekick.Avalonia;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (OperatingSystem.IsLinux())
        {
            // Disable the modern hardware-accelerated buffer sharing layout
            // This is the true culprit behind the black screen behavior
            Environment.SetEnvironmentVariable("WEBKIT_DISABLE_DMABUF_RENDERER", "1");
        }
        
        VelopackApp.Build().Run();
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }
}
