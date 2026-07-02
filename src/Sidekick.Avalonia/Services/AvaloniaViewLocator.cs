using Avalonia;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator(ILogger<AvaloniaViewLocator> logger) : IViewLocator
{

    public SidekickViewType LastOpenedType { get; private set; }

    private StandardWindow? StandardWindow { get; set; }
    private OverlayWindow? OverlayWindow { get; set; }
    private SplashWindow? SplashWindow { get; set; }

    private Application GetApplication() => Application.Current ?? throw new Exception("Application is not initialized.");

    public void Open(SidekickViewType type, string url)
    {
        LastOpenedType = type;

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
                    OverlayWindow ??= new OverlayWindow();
                    await OverlayWindow.OpenView(uri.ToString());
                    break;
                case SidekickViewType.Splash:
                    SplashWindow ??= new SplashWindow();
                    SplashWindow.OpenView(uri.ToString());
                    break;
                default:
                    StandardWindow ??= new StandardWindow();
                    await StandardWindow.OpenView(uri.ToString());
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
                    StandardWindow?.CloseView();
                    break;
            }
        });
    }

    public void Maximize(SidekickViewType type)
    {
        GetApplication().Dispatcher.Invoke(() =>
        {
            switch (type)
            {
                case SidekickViewType.Standard:
                    StandardWindow?.MaximizeView();
                    break;
                default:
                    throw new NotImplementedException();
            }
        });
    }

    public void Minimize(SidekickViewType type)
    {
        GetApplication().Dispatcher.Invoke(() =>
        {
            switch (type)
            {
                case SidekickViewType.Standard:
                    StandardWindow?.MinimizeView();
                    break;
                default:
                    throw new NotImplementedException();
            }
        });
    }

    public bool IsOpened(SidekickViewType type)
    {
        return type switch
        {
            SidekickViewType.Overlay => OverlayWindow?.IsVisible ?? false,
            SidekickViewType.Splash => SplashWindow?.IsVisible ?? false,
            _ => StandardWindow?.IsVisible ?? false,
        };
    }
}
