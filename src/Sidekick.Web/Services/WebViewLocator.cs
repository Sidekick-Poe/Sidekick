using Sidekick.Common.Ui.Views;
using Sidekick.Web.Pages;

namespace Sidekick.Web.Services;

public class WebViewLocator : IViewLocator
{
    public List<SidekickWebWrapper> Views { get; } = [];

    public SidekickViewType LastOpenedType => SidekickViewType.Standard;

    public void Open(SidekickViewType type, string url)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo(url));
    }

    public void Close(SidekickViewType type)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo("/home"));
    }

    public void Maximize(SidekickViewType type)
    {
    }

    public void Minimize(SidekickViewType type)
    {
    }

    public void StartMoving(SidekickViewType type, int pageX, int pageY)
    {
    }

    public void StopMoving(SidekickViewType type)
    {
    }

    public bool IsOpened(SidekickViewType type) => false;
}
