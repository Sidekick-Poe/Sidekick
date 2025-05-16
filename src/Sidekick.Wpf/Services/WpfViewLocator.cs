using System.Globalization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Localization;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfViewLocator : IViewLocator, IDisposable
{
    private readonly IUiLanguageProvider uiLanguageProvider;

    public WpfViewLocator(ILogger<WpfViewLocator> logger, IUiLanguageProvider uiLanguageProvider)
    {
        this.uiLanguageProvider = uiLanguageProvider;
        this.uiLanguageProvider.OnLanguageChanged += SetCultureInfo;

        StandardWindow =  System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Standard, logger));
        OverlayWindow =  System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Overlay, logger));
        ModalWindow = System.Windows.Application.Current.Dispatcher.Invoke(() => new MainWindow(SidekickViewType.Modal, logger));

        SetCultureInfo();
    }

    private async void SetCultureInfo(CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= await uiLanguageProvider.Get();

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        });
    }

    public bool SupportsMinimize => true;

    public bool SupportsMaximize => true;

    private MainWindow StandardWindow { get; }

    private MainWindow OverlayWindow { get; }

    private MainWindow ModalWindow { get; }

    public void Open(SidekickViewType type, string url)
    {
        var window = GetWindow(type);
        _ = window.OpenView(url);
    }

    public MainWindow GetWindow(SidekickViewType type)
    {
        return type switch
        {
            SidekickViewType.Standard => StandardWindow,
            SidekickViewType.Overlay => OverlayWindow,
            SidekickViewType.Modal => ModalWindow,
            _ => throw new SidekickException("The window could not be determined.")
        };
    }

    public void Close(SidekickViewType type)
    {
        var window = GetWindow(type);

        if (window.View != null) window.View.Close();
        else window.CloseView();
    }

    public bool IsOverlayOpened()
    {
        return OverlayWindow.IsVisible;
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
        StandardWindow.Dispose();
        OverlayWindow.Dispose();
        ModalWindow.Dispose();
    }
}
