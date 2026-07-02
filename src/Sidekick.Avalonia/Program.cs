using Avalonia;
using Velopack;

namespace Sidekick.Avalonia;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // It's important to Run() the VelopackApp as early as possible in app startup.
        VelopackApp.Build().Run();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
