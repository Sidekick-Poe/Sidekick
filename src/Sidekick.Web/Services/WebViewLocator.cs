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
        Views.ForEach(x => x.NavigationManager.NavigateTo("/item"));
    }

    public bool IsOpened(SidekickViewType type) => false;

    /// <summary>
    /// Does nothing, since the browser decides the size of the page.
    /// </summary>
    public void SetSize(SidekickViewType type, int width, int height, object owner) { }

    /// <summary>
    /// Does nothing, since the browser decides the size of the page.
    /// </summary>
    public void ResetSize(SidekickViewType type, object owner) { }
}
