using System.Globalization;
using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Localization;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaViewLocator : IViewLocator, IDisposable
{
    private readonly ILogger<AvaloniaViewLocator> logger;
    private readonly IUiLanguageProvider uiLanguageProvider;

    public AvaloniaViewLocator(ILogger<AvaloniaViewLocator> logger, IUiLanguageProvider uiLanguageProvider)
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

            await Dispatcher.UIThread.InvokeAsync(() =>
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

    private Dictionary<SidekickViewType, MainWindow> Windows { get; } = [];

    public void Open(SidekickViewType type, string url)
    {
        // Ensure we're on the UI thread to access windows
        if (Dispatcher.UIThread.CheckAccess())
        {
            var window = GetWindowSync(type, true);
            if (window != null)
            {
                _ = window.OpenView(url);
            }
        }
        else
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var window = GetWindowSync(type, true);
                if (window != null)
                {
                    _ = window.OpenView(url);
                }
            });
        }
    }

    private MainWindow? GetWindowSync(SidekickViewType type, bool create)
    {
        var window = Windows.GetValueOrDefault(type);
        if (window != null) return window;
        if (!create) return null;

        window = new MainWindow(type);
        Windows.Add(type, window);
        return window;
    }

    public void Close(SidekickViewType type)
    {
        var window = GetWindowSync(type, false);
        if (window == null) return;

        _ = window.CloseView();
    }

    public bool IsOverlayOpened()
    {
        var window = GetWindowSync(SidekickViewType.Overlay, false);
        return window?.IsVisible ?? false;
    }

    public void MinimizeWindow(SidekickViewType type)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var window = GetWindowSync(type, false);
            if (window != null) window.WindowState = WindowState.Minimized;
        });
    }

    public void MaximizeWindow(SidekickViewType type)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var window = GetWindowSync(type, false);
            if (window != null)
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        });
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
