using System.Globalization;
using Avalonia;
using Sidekick.Common.Localization;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator : IViewLocator, IDisposable
{
    private readonly ILogger<AvaloniaViewLocator> logger;
    private readonly IUiLanguageProvider uiLanguageProvider;

    private StandardWindow? MainWindow { get; set; }
    private OverlayWindow? OverlayWindow { get; set; }
    private SplashWindow? SplashWindow { get; set; }

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
        GetApplication().Dispatcher.InvokeAsync(async () =>
        {
            var host = App.ServerAppHost.Application.Urls.FirstOrDefault();
            if (host == null)
            {
                logger.LogCritical("[AvaloniaViewLocator] No host found.");
                throw new Exception("No host found.");
            }

            var uri = new Uri(new Uri(host), url);

            switch (type)
            {
                case SidekickViewType.Overlay:
                    OverlayWindow ??= new OverlayWindow(App.ServerAppHost.Application.Services);
                    await OverlayWindow.OpenView(uri.ToString());
                    break;
                case SidekickViewType.Splash:
                    SplashWindow ??= new SplashWindow();
                    SplashWindow.OpenView(uri.ToString());
                    break;
                default:
                    MainWindow ??= new StandardWindow(App.ServerAppHost.Application.Services);
                    await MainWindow.OpenView(uri.ToString());
                    break;
            }
        });
    }

    public void Close(SidekickViewType type)
    {
        GetApplication().Dispatcher.Invoke(() =>
        {
            switch (type)
            {
                case SidekickViewType.Overlay:
                    OverlayWindow?.CloseView();
                    break;
                case SidekickViewType.Splash:
                    SplashWindow?.CloseView();
                    break;
                default:
                    MainWindow?.CloseView();
                    break;
            }
        });
    }

    public bool IsOpened(SidekickViewType type)
    {
        return type switch
        {
            SidekickViewType.Overlay => OverlayWindow?.IsVisible ?? false,
            SidekickViewType.Splash => SplashWindow?.IsVisible ?? false,
            _ => MainWindow?.IsVisible ?? false,
        };
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
    }
}
