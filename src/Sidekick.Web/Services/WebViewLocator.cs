using Sidekick.Common.Ui.Views;

namespace Sidekick.Web.Services;

public class WebViewLocator : IViewLocator
{
    public bool SupportsMinimize => false;

    public bool SupportsMaximize => false;

    private SidekickView? LastView { get; set; }

    public Task Open(string url)
    {
        LastView?.NavigationManager.NavigateTo(url);
        return Task.CompletedTask;
    }

    public Task CloseAll()
    {
        throw new NotImplementedException();
    }

    public Task CloseAllOverlays()
    {
        throw new NotImplementedException();
    }

    public bool IsOverlayOpened()
    {
        return false;
    }

    public Task Close(SidekickView view)
    {
        view.NavigationManager.NavigateTo("/home");
        return Task.CompletedTask;
    }

    public Task Initialize(SidekickView view)
    {
        LastView = view;
        return Task.CompletedTask;
    }

    public Task Minimize(SidekickView view)
    {
        throw new NotImplementedException();
    }

    public Task Maximize(SidekickView view)
    {
        throw new NotImplementedException();
    }
}
