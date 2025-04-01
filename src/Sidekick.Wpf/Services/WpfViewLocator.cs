using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfViewLocator(ILogger<WpfViewLocator> logger) : IViewLocator, IDisposable
{
    public bool SupportsMinimize => true;

    public bool SupportsMaximize => true;

    private MainWindow? StandardWindow { get; set; }

    private MainWindow? OverlayWindow { get; set; }

    private MainWindow? ModalWindow { get; set; }

    public void Open(SidekickViewType type, string url)
    {
        var window = GetWindow(type);
        window.OpenView(url);
    }

    public MainWindow GetWindow(SidekickViewType type)
    {
        switch (type)
        {
            case SidekickViewType.Standard:
                StandardWindow ??= System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Standard, logger));
                return StandardWindow;

            case SidekickViewType.Overlay:
                OverlayWindow ??= System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Overlay, logger));
                return OverlayWindow;

            case SidekickViewType.Modal:
                ModalWindow ??= System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Modal, logger));
                return ModalWindow;

            default: throw new SidekickException("The window could not be determined.");
        }
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
        return OverlayWindow?.IsVisible ?? false;
    }

    public void Dispose()
    {
        StandardWindow?.Dispose();
        OverlayWindow?.Dispose();
        ModalWindow?.Dispose();
    }
}
