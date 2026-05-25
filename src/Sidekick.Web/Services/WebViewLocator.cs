using Sidekick.Common.Ui.Views;
using Sidekick.Web.Pages;

namespace Sidekick.Web.Services;

public class WebViewLocator : IViewLocator
{
    public List<SidekickWebWrapper> Views { get; } = [];

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
