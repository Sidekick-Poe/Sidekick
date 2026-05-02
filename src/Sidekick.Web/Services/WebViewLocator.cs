using Sidekick.Common.Ui.Views;

namespace Sidekick.Web.Services;

public class WebViewLocator : IViewLocator
{
    /// <inheritdoc />
    public bool SupportsMinimize => false;

    /// <inheritdoc />
    public bool SupportsMaximize => false;

    public bool SupportsAlwaysOnTop => false;

    public List<SidekickWebWrapper> Views { get; } = [];

    public bool IsOverlayOpened() => false;

    public void Open(string url, bool alwaysOnTop)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo(url));
    }

    public void Close()
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo("/home"));
    }
}
