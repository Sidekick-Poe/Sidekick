using Sidekick.Common.Ui.Views;
using Sidekick.Electron.Components;
namespace Sidekick.Electron.Services;

public class ElectronViewLocator : IViewLocator
{
    public ElectronViewLocator()
    {
        
    }

    /// <inheritdoc />
    public bool SupportsMinimize => true;

    /// <inheritdoc />
    public bool SupportsMaximize => true;

    public List<SidekickElectronBlazorWrapper> Views { get; } = [];

    public void Open(SidekickViewType type, string url)
    {
        Views.ForEach(x => x.NavigationManager.NavigateTo(url));
    }

    public void Close(SidekickViewType type)
    {
        var a = ElectronNET.API.Electron.WindowManager.BrowserWindows.ToList();
        Views.ForEach(x => x.NavigationManager.NavigateTo("/home"));
    }

    public bool IsOverlayOpened() => false;
}
