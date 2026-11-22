using Sidekick.Avalonia.Controls;
using Avalonia;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator(
    ILogger<AvaloniaViewLocator> logger) : IViewLocator
{

    public SidekickViewType LastOpenedType { get; private set; }

    private StandardWindow? StandardWindow { get; set; }
    private OverlayWindow? OverlayWindow { get; set; }
    private SplashWindow? SplashWindow { get; set; }

    /// <summary>
    /// Window visibility, kept up to date on the UI thread, so <see cref="IsOpened"/> answers from any thread without
    /// waiting on the UI thread. Hotkey checks run from the keyboard hook and, on Linux, while the keyboard is frozen.
    /// </summary>
    private volatile bool isStandardVisible;

    private volatile bool isOverlayVisible;

    private volatile bool isSplashVisible;

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
                    OverlayWindow ??= Track(new OverlayWindow(), window => OverlayWindow = window, visible => isOverlayVisible = visible);
                    OverlayWindow.CancelRelease();
                    await OverlayWindow.OpenView(uri.ToString());
                    break;
                case SidekickViewType.Splash:
                    SplashWindow ??= Track(new SplashWindow(), window => SplashWindow = window, visible => isSplashVisible = visible);
                    SplashWindow.CancelRelease();
                    await SplashWindow.OpenView(uri.ToString());
                    break;
                default:
                    StandardWindow ??= Track(new StandardWindow(), window => StandardWindow = window, visible => isStandardVisible = visible);
                    StandardWindow.CancelRelease();
                    await StandardWindow.OpenView(uri.ToString());
                    break;
            }
        });
    }

    /// <summary>
    /// Follows the window's visibility, and forgets the window once it is released, so that the next open creates a new one.
    /// </summary>
    private TWindow Track<TWindow>(TWindow window, Action<TWindow?> setWindow, Action<bool> setVisible)
        where TWindow : BaseWindow
    {
        window.PropertyChanged += (_, e) =>
        {
            if (e.Property == Visual.IsVisibleProperty)
            {
                setVisible(window.IsVisible);
            }
        };
        window.Closed += (_, _) =>
        {
            setVisible(false);
            setWindow(null);
            ScheduleMemoryTrim();
        };
        return window;
    }

    private CancellationTokenSource? trimCancellation;

    /// <summary>
    /// Returns the memory freed by a released window to the system once its page is gone.
    /// Blazor keeps a disconnected page alive for the circuit retention period, so the trim waits for it to be disposed first.
    /// </summary>
    private void ScheduleMemoryTrim()
    {
        trimCancellation?.Cancel();
        trimCancellation = new CancellationTokenSource();
        _ = TrimMemoryAfterDelay(trimCancellation.Token);
    }

    private async Task TrimMemoryAfterDelay(CancellationToken cancellationToken)
    {
        try
        {
            var circuitOptions = App.RequiredServerAppHost.Application.Services.GetRequiredService<IOptions<CircuitOptions>>();
            await Task.Delay(circuitOptions.Value.DisconnectedCircuitRetentionPeriod + TimeSpan.FromSeconds(30), cancellationToken);

            // The check and the collection run in the same UI thread call, so a window opened by a hotkey waits for the collection instead of being paused by it.
            await GetApplication().Dispatcher.InvokeAsync(() =>
            {
                if (OverlayWindow?.IsVisible == true || StandardWindow?.IsVisible == true || SplashWindow?.IsVisible == true)
                {
                    return;
                }

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
            });
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "[AvaloniaViewLocator] Could not trim the memory.");
        }
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

    public bool IsOpened(SidekickViewType type)
    {
        return type switch
        {
            SidekickViewType.Overlay => isOverlayVisible,
            SidekickViewType.Splash => isSplashVisible,
            _ => isStandardVisible,
        };
    }

    public void SetSize(SidekickViewType type, int width, int height, object owner)
    {
        GetApplication().Dispatcher.InvokeAsync(async () =>
        {
            var window = GetWindow(type);
            if (window != null) await window.SetRequestedSize(width, height, owner);
        });
    }

    public void ResetSize(SidekickViewType type, object owner)
    {
        GetApplication().Dispatcher.InvokeAsync(async () =>
        {
            var window = GetWindow(type);
            if (window != null) await window.ResetRequestedSize(owner);
        });
    }

    private BaseWindow? GetWindow(SidekickViewType type) => type switch
    {
        SidekickViewType.Overlay => OverlayWindow,
        SidekickViewType.Splash => SplashWindow,
        _ => StandardWindow,
    };
}
