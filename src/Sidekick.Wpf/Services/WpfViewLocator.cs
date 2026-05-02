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

    public bool SupportsMinimize => true;

    public bool SupportsMaximize => true;

    public bool SupportsAlwaysOnTop => true;

    private MainWindow? Window { get; set; }

    public void Open(string url, bool alwaysOnTop)
    {
        var window = GetWindow(true)!;
        _ = window.OpenView(url, alwaysOnTop);
    }

    public bool IsOverlayOpened()
    {
        throw new NotImplementedException();
    }

    private MainWindow? GetWindow(bool create)
    {
        if (Window != null) return Window;
        if (!create) return null;

        return System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Window = new MainWindow(logger);
            return Window;
        }).Result;
    }

    public void Close()
    {
        var window = GetWindow(false);
        if (window == null) return;

        window.CloseView();
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
        Window?.Dispose();
    }
}