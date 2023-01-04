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
        if (window != null)
        {
            window.Title = "Sidekick";
        }

        return window;
    }
}
