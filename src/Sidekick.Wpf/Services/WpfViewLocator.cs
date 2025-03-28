using Microsoft.Extensions.Logging;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfViewLocator(ILogger<WpfViewLocator> logger) : IViewLocator, IDisposable
{
    public bool SupportsMinimize => true;

    public bool SupportsMaximize => true;

    private MainWindow StandardWindow { get; } = new(SidekickViewType.Standard, logger);

    private MainWindow OverlayWindow { get; } = new(SidekickViewType.Overlay, logger);

    private MainWindow ModalWindow { get; } = new(SidekickViewType.Modal, logger);

    public void Open(SidekickViewType type, string url)
    {
        var window = GetWindow(type);
        window.OpenView(url);
    }

    public MainWindow GetWindow(SidekickViewType type)
    {
        return type switch
        {
            SidekickViewType.Standard => StandardWindow,
            SidekickViewType.Overlay => OverlayWindow,
            SidekickViewType.Modal => ModalWindow,
            _ => throw new SidekickException("The window could not be determined."),
        };
    }

    public void Close(SidekickViewType type)
    {
        var window = GetWindow(type);
        if (window.View != null)
        {
            window.View.Close();
        }
        else
        {
            window.CloseView();
        }
    }

    public bool IsOverlayOpened()
    {
        return OverlayWindow.IsVisible;
    }

    public void Dispose()
    {
        StandardWindow.Dispose();
        OverlayWindow.Dispose();
        ModalWindow.Dispose();
    }
}
