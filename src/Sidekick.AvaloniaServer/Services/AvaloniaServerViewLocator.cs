using Sidekick.Common.Ui.Views;

namespace Sidekick.AvaloniaServer.Services;

/// <summary>
/// View locator for Blazor Server running in Avalonia.
/// Delegates window show/hide to the Avalonia host's view locator,
/// and navigates Blazor components to the appropriate URL.
/// </summary>
public class AvaloniaServerViewLocator(IViewLocator avaloniaViewLocator, IAvaloniaWindowHost windowHost) : IViewLocator
{
    public bool SupportsMinimize => true;

    public bool SupportsMaximize => true;

    public List<SidekickAvaloniaServerWrapper> Views { get; } = [];

    public void Open(SidekickViewType type, string url)
    {
        // Show/position the Avalonia window
        avaloniaViewLocator.Open(type, url);

        // Navigate the Blazor components to the URL
        foreach (var view in Views)
        {
            view.NavigationManager.NavigateTo(url);
        }
    }

    public void Close(SidekickViewType type)
    {
        // Hide the Avalonia window
        avaloniaViewLocator.Close(type);

        // Navigate Blazor components back to home
        foreach (var view in Views)
        {
            view.NavigationManager.NavigateTo("/home");
        }
    }

    public bool IsOverlayOpened() => avaloniaViewLocator.IsOverlayOpened();

    /// <summary>Called by SidekickAvaloniaServerWrapper when ICurrentView.Minimized fires.</summary>
    public void MinimizeWindow(SidekickViewType type) => windowHost.MinimizeWindow(type);

    /// <summary>Called by SidekickAvaloniaServerWrapper when ICurrentView.Maximized fires.</summary>
    public void MaximizeWindow(SidekickViewType type) => windowHost.MaximizeWindow(type);
}
