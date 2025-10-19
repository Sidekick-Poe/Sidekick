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

    private Dictionary<SidekickViewType, MainWindow> Windows { get; } = [];

    public void Open(SidekickViewType type, string url)
    {
        var window = GetWindow(type, true)!;
        _ = window.OpenView(url);
    }

    private MainWindow? GetWindow(SidekickViewType type, bool create)
    {
        var window = Windows.GetValueOrDefault(type);
        if (window != null) return window;
        if (!create) return null;

        window = new MainWindow(type, logger);
        Windows.Add(type, window);
        return window;
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
        var window = GetWindow(SidekickViewType.Overlay, false);
        return window?.IsVisible ?? false;
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;

        foreach (var window in Windows)
        {
            window.Value.Dispose();
        }
    }
}
