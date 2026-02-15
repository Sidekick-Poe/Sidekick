using Sidekick.Common.Ui.Views;
using Sidekick.Electron.Components;
namespace Sidekick.Electron.Services;

public class ElectronViewLocator : IViewLocator
{
    /// <inheritdoc />
    public bool SupportsMinimize => false;

    /// <inheritdoc />
    public bool SupportsMaximize => false;

    public List<SidekickElectronBlazorWrapper> Views { get; } = [];

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
