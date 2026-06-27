using System.Globalization;
using Microsoft.Extensions.Logging;
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
        try
        {
            cultureInfo ??= await uiLanguageProvider.Get();

            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            });
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Error while trying to set culture info.");
        }
    }

    private MainWindow? Window { get; set; }

    public void Open(SidekickViewType type, string url)
    {
        _ = GetOrCreateWindow().OpenView(type, url);
        return;

        MainWindow GetOrCreateWindow()
        {
            if (Window != null) return Window;

            return System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Window = new MainWindow(logger);
                return Window;
            }).Result;
        }
    }

    public void Close()
    {
        Window?.CloseView();
    }

    public bool IsOverlayOpened()
    {
        return Window?.IsVisible ?? false;
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
        Window?.Dispose();
    }
}
