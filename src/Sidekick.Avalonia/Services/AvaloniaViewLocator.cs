using Avalonia;
using Avalonia.Controls;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.EventArgs;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator(
    ILogger<AvaloniaViewLocator> logger,
    IInputProvider inputProvider) : IViewLocator
{

    public SidekickViewType LastOpenedType { get; private set; }

    private StandardWindow? StandardWindow { get; set; }
    private OverlayWindow? OverlayWindow { get; set; }
    private SplashWindow? SplashWindow { get; set; }

    private Action<DraggedEventArgs>? OnMouseDrag { get; set; }

    private Application GetApplication() => Application.Current ?? throw new Exception("Application is not initialized.");

    public void Open(SidekickViewType type, string url)
    {
        LastOpenedType = type;

        GetApplication().Dispatcher.InvokeAsync(async () =>
        {
            var host = App.RequiredServerAppHost.Application.Urls.FirstOrDefault();
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
                    await SplashWindow.OpenView(uri.ToString());
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
        return GetApplication().Dispatcher.Invoke(() =>
        {
            return type switch
            {
                SidekickViewType.Overlay => OverlayWindow?.IsVisible ?? false,
                SidekickViewType.Splash => SplashWindow?.IsVisible ?? false,
                _ => StandardWindow?.IsVisible ?? false,
            };
        });
    }


    public void StartMoving(SidekickViewType type, int pageX, int pageY)
    {
        StopMoving(type);

        OnMouseDrag = args =>
        {
            GetApplication().Dispatcher.Invoke(() =>
            {
                Window? window = type switch
                {
                    SidekickViewType.Overlay => OverlayWindow,
                    SidekickViewType.Splash => SplashWindow,
                    _ => StandardWindow,
                };
                if (window == null || !window.IsVisible || window.WindowState == WindowState.Maximized) return;

                window.Position = new PixelPoint(args.X - pageX, args.Y - pageY);
            });
        };

        inputProvider.OnMouseDrag += OnMouseDrag;
    }

    public void StopMoving(SidekickViewType type)
    {
        if (OnMouseDrag == null) return;

        inputProvider.OnMouseDrag -= OnMouseDrag;
        OnMouseDrag = null;
    }
}
