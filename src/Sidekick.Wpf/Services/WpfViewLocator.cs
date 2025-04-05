using System.Globalization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Localization;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfViewLocator : IViewLocator, IDisposable
{
    private readonly ILogger<WpfViewLocator> logger;
    private readonly IUiLanguageProvider uiLanguageProvider;

    public WpfViewLocator(ILogger<WpfViewLocator> logger, IUiLanguageProvider uiLanguageProvider)
    {
        this.logger = logger;
        this.uiLanguageProvider = uiLanguageProvider;
        this.uiLanguageProvider.OnLanguageChanged += SetCultureInfo;

        SetCultureInfo();
    }

    private async void SetCultureInfo(CultureInfo? cultureInfo = null)
    {
        if (cultureInfo == null)
        {
            cultureInfo = await uiLanguageProvider.Get();
        }

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        });
    }

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
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
        StandardWindow?.Dispose();
        OverlayWindow?.Dispose();
        ModalWindow?.Dispose();
    }
}
