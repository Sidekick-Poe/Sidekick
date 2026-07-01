using System.Globalization;
using Avalonia;
using Sidekick.Common.Localization;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator : IViewLocator, IDisposable
{
    private readonly ILogger<AvaloniaViewLocator> logger;
    private readonly IUiLanguageProvider uiLanguageProvider;

    public MainWindow? MainWindow { get; set; }
    private OverlayWindow? OverlayWindow { get; set; }

    public AvaloniaViewLocator(ILogger<AvaloniaViewLocator> logger, IUiLanguageProvider uiLanguageProvider)
    {
        this.logger = logger;
        this.uiLanguageProvider = uiLanguageProvider;
        this.uiLanguageProvider.OnLanguageChanged += SetCultureInfo;

        SetCultureInfo();
    }

    private Application GetApplication() => Application.Current ?? throw new Exception("Application is not initialized.");

    private async void SetCultureInfo(CultureInfo? cultureInfo = null)
    {
        try
        {
            cultureInfo ??= await uiLanguageProvider.Get();

            await GetApplication().Dispatcher.InvokeAsync(() =>
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

    public void Open(SidekickViewType type, string url)
    {
        switch (type)
        {
            case SidekickViewType.Overlay:
                GetApplication().Dispatcher.InvokeAsync(async () =>
                {
                    OverlayWindow ??= new OverlayWindow(App.ServerAppHost.Application.Services);
                    await OverlayWindow.OpenView(url);
                });
                break;
            default:
                GetApplication().Dispatcher.InvokeAsync(async () =>
                {
                    MainWindow ??= new MainWindow(App.ServerAppHost.Application.Services);
                    await MainWindow.OpenView(url);
                });
                break;
        }
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public bool IsOverlayOpened()
    {
        return OverlayWindow?.IsVisible ?? false;
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
        MainWindow?.Dispose();
        OverlayWindow?.Dispose();
    }
}
