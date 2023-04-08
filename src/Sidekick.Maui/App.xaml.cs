namespace Sidekick.Maui;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        var initialPage = "/update";

#if DEBUG
        initialPage = "/setup";
#endif

        InitializeComponent();
        MainPage = new InitialWindow(serviceProvider, initialPage);
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Title = "Sidekick";

        window.Width = 400;
        window.MinimumWidth = 400;
        window.MaximumWidth = 400;

        window.Height = 260;
        window.MinimumHeight = 260;
        window.MaximumHeight = 260;

        return window;
    }

    public override void CloseWindow(Window window)
    {
        base.CloseWindow(window);
    }
}
