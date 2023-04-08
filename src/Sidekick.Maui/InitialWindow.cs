namespace Sidekick.Maui;

public class InitialWindow : BlazorWindow
{
    public InitialWindow(IServiceProvider serviceProvider, string initialUrl)
        : base(serviceProvider, initialUrl)
    {
    }
}
