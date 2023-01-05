using Sidekick.Common.Blazor.Views;

namespace Sidekick.Maui;

public partial class App : Application
{
    public App(IViewLocator viewLocator, IServiceProvider serviceProvider)
    {
        var initialPage = "/update";

#if DEBUG
        initialPage = "/setup";
#endif

        InitializeComponent();
        MainPage = new BlazorWindow(serviceProvider, initialPage);
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
}
