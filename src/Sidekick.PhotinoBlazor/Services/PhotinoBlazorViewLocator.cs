using Sidekick.Common.Ui.Views;

namespace Sidekick.PhotinoBlazor.Services;

public class PhotinoBlazorViewLocator : IViewLocator
{
    /// <inheritdoc />
    public bool SupportsMinimize => true;

    /// <inheritdoc />
    public bool SupportsMaximize => true;

    public List<SidekickPhotinoBlazorWrapper> Views { get; } = [];

    public void Open(SidekickViewType type, string url)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo(url));
    }

    public void Close(SidekickViewType type)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo("/home"));
    }

    public bool IsOverlayOpened() => false;
}
