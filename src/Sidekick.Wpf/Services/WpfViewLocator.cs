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
        var window = GetWindow(type, true);
        if (window != null) window.OpenView(url);
    }

    public MainWindow? GetWindow(SidekickViewType type, bool createIfMissing)
    {
        switch (type)
        {
            case SidekickViewType.Standard:
                if (createIfMissing)
                {
                    StandardWindow ??= System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Standard, logger));
                }

                return StandardWindow;

            case SidekickViewType.Overlay:
                if (createIfMissing)
                {
                    OverlayWindow ??= System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Overlay, logger));
                }

                return OverlayWindow;

            case SidekickViewType.Modal:
                if (createIfMissing)
                {
                    ModalWindow ??= System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Modal, logger));
                }

                return ModalWindow;

            default: throw new SidekickException("The window could not be determined.");
        }
    }

    public void Close(SidekickViewType type)
    {
        var window = GetWindow(type, false);
        if (window == null) return;

        if (window.View != null)
            window.View.Close();
        else
            window.CloseView();
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
