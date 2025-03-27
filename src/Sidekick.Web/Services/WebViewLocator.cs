using Sidekick.Common.Ui.Views;

namespace Sidekick.Web.Services;

public class WebViewLocator : IViewLocator
{
    /// <inheritdoc />
    public bool SupportsMinimize => false;

    /// <inheritdoc />
    public bool SupportsMaximize => false;

    private List<ICurrentView> Views { get; } = [];

    /// <inheritdoc />
    public Task Open(SidekickViewType type, string url)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo(url));
        return Task.CompletedTask;
    }

    public Task CloseStandard()
    {
        throw new NotImplementedException();
    }

    public Task CloseOverlay()
    {
        throw new NotImplementedException();
    }

    public bool IsOverlayOpened()
    {
        return false;
    }

    public Task Close(ICurrentView view)
    {
        view.NavigationManager.NavigateTo("/home");
        return Task.CompletedTask;
    }

    public Task Initialize(ICurrentView view)
    {
        Views.Add(view);
        return Task.CompletedTask;
    }

    public Task Minimize(ICurrentView view)
    {
        throw new NotImplementedException();
    }

    public Task Maximize(ICurrentView view)
    {
        throw new NotImplementedException();
    }

    public void ViewClosed(ICurrentView view)
    {
        Views.Remove(view);
    }
}
