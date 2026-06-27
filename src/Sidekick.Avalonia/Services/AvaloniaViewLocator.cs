using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator : IViewLocator
{
    public void Open(SidekickViewType type, string url)
    {
    }

    public void Close()
    {
    }

    public bool IsOverlayOpened()
    {
        return false;
    }
}
