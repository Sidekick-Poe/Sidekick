using Sidekick.Maui.Services;

namespace Sidekick.Maui;

public partial class BlazorWindow : ContentPage
{
    public BlazorWindow(IServiceProvider serviceProvider, string initialUrl)
    {
        var locator = serviceProvider.GetService<ViewLocator>();
        locator.SetNextWindow(this, initialUrl);

        InitializeComponent();
    }
}
