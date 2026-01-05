using Sidekick.Common.Ui.Views;

namespace Sidekick.Linux.Platform;

public class X11ViewLocator : IViewLocator
{
    private int openOverlayCount;

    public Task Open(string url)
    {
        // TODO: Create an X11 overlay window and host WebKitGTK with the given URL.
        if (!string.IsNullOrEmpty(url))
        {
            openOverlayCount = Math.Max(openOverlayCount, 1);
        }

        return Task.CompletedTask;
    }

    public Task Initialize(SidekickView view)
    {
        // TODO: Initialize the native window for the view.
        return Task.CompletedTask;
    }

    public Task Minimize(SidekickView view)
    {
        // TODO: Minimize or hide the native window.
        return Task.CompletedTask;
    }

    public Task Maximize(SidekickView view)
    {
        // TODO: Resize the native window to the target bounds.
        return Task.CompletedTask;
    }

    public Task Close(SidekickView view)
    {
        // TODO: Destroy the native window.
        openOverlayCount = Math.Max(0, openOverlayCount - 1);
        return Task.CompletedTask;
    }

    public Task CloseAll()
    {
        // TODO: Close all open windows.
        openOverlayCount = 0;
        return Task.CompletedTask;
    }

    public Task CloseAllOverlays()
    {
        // TODO: Close overlay windows only.
        openOverlayCount = 0;
        return Task.CompletedTask;
    }

    public bool IsOverlayOpened() => openOverlayCount > 0;
}
