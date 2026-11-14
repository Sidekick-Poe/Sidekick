using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Sidekick.Common.Settings;

#if WINDOWS
using Win32Interop = Sidekick.Avalonia.Utilities.WindowsNativeMethods;
#endif

namespace Sidekick.Avalonia.Controls;

public abstract class BaseWindow : Window
{
    protected IServiceProvider ServiceProvider => App.RequiredServerAppHost.Application.Services;

    private int DefaultHeight { get; }
    private int DefaultWidth { get; }
    private string? PositionPrefix { get; }

    protected BaseWindow(int defaultHeight, int defaultWidth, string? positionPrefix = null) : base()
    {
        DefaultHeight = defaultHeight;
        DefaultWidth = defaultWidth;
        PositionPrefix = positionPrefix;

        Width = DefaultWidth;
        Height = DefaultHeight;
        MinWidth = DefaultWidth;
        MinHeight = DefaultHeight;

        Title = "Sidekick";
        RequestedThemeVariant = ThemeVariant.Dark;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaTitleBarHeightHint = -1;
        Background = SolidColorBrush.Parse("#1C1917");
        Opened += BaseWindow_Opened;
        PositionChanged += BaseWindow_PositionChanged;
    }

    private void BaseWindow_Opened(object? sender, EventArgs e)
    {
#if WINDOWS
        // This code block is entirely omitted on Linux/macOS builds
        var platformHandle = TryGetPlatformHandle();
        if (platformHandle != null)
        {
            IntPtr hWnd = platformHandle.Handle;
            int useDarkMode = 1;
            Win32Interop.DwmSetWindowAttribute(hWnd, 20, ref useDarkMode, sizeof(int));

            int borderColor = unchecked((int)0xFFFFFFFE);
            Win32Interop.DwmSetWindowAttribute(hWnd, 34, ref borderColor, sizeof(int));
        }
#endif
    }

    public void CloseView()
    {
        Dispatcher.Invoke(() =>
        {
            // Save the final placement now, since a hidden window cannot be measured anymore.
            var placement = GetPlacement();
            saveCancellation?.Cancel();
            Hide();
            IsPlaced = false;
            _ = SavePlacement(placement);
        });
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        CloseView();
        e.Cancel = true;
        base.OnClosing(e);
    }

    /// <summary>
    /// Sizes and places the window after it is shown: always the saved size, and the saved position when the setting is enabled, otherwise centered on the screen.
    /// A window that is already open and placed is left where the user put it.
    /// </summary>
    protected async Task NormalizeView()
    {
        WindowState = WindowState.Normal;
        if (IsPlaced && IsVisible) return;

        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        var width = DefaultWidth * zoom;
        var height = DefaultHeight * zoom;
        MinWidth = width;
        MinHeight = height;

        PixelPoint? savedPosition = null;
        if (!string.IsNullOrEmpty(PositionPrefix))
        {
            // The size is always remembered. The setting only controls the position.
            var savedWidth = await settingsService.GetDouble($"{PositionPrefix}_Width");
            var savedHeight = await settingsService.GetDouble($"{PositionPrefix}_Height");
            if (savedWidth >= width && savedHeight >= height)
            {
                width = savedWidth;
                height = savedHeight;
            }

            // A saved width of 0 means that no placement was saved yet.
            if (savedWidth > 0 && await settingsService.GetBool(SettingKeys.SaveWindowPositions))
            {
                var positionX = await settingsService.GetInt($"{PositionPrefix}_PositionX");
                var positionY = await settingsService.GetInt($"{PositionPrefix}_PositionY");
                savedPosition = new PixelPoint(positionX, positionY);
            }
        }

        Width = width;
        Height = height;

        var screen = (savedPosition != null ? Screens.ScreenFromPoint(savedPosition.Value) : null)
                     ?? Screens.ScreenFromWindow(this)
                     ?? Screens.Primary;
        if (screen != null)
        {
            // Width and Height are in device-independent units, while positions are in physical pixels.
            var workingArea = screen.WorkingArea;
            var pixelWidth = (int)Math.Ceiling(width * screen.Scaling);
            var pixelHeight = (int)Math.Ceiling(height * screen.Scaling);

            var position = savedPosition ?? new PixelPoint(
                workingArea.X + (workingArea.Width - pixelWidth) / 2,
                workingArea.Y + (workingArea.Height - pixelHeight) / 2);

            // Keep the window within the screen.
            var maxX = Math.Max(workingArea.X, workingArea.Right - pixelWidth);
            var maxY = Math.Max(workingArea.Y, workingArea.Bottom - pixelHeight);
            Position = new PixelPoint(
                Math.Clamp(position.X, workingArea.X, maxX),
                Math.Clamp(position.Y, workingArea.Y, maxY));
        }

        // Remember the placement we just applied, so that it is not saved again unless the user changes it.
        lastSavedPlacement = new WindowPlacement(Width, Height, Position.X, Position.Y);
        IsPlaced = true;
    }

    #region Save Placement

    private static readonly SemaphoreSlim SavePlacementSemaphore = new(1, 1);

    private CancellationTokenSource? saveCancellation;

    private WindowPlacement? lastSavedPlacement;

    private bool IsPlaced { get; set; }

    private record WindowPlacement(double Width, double Height, int X, int Y);

    protected override void OnResized(WindowResizedEventArgs e)
    {
        // Only resizes made by the user are saved, not the ones made by the code (placement, zoom, layout).
        if (e.Reason == WindowResizeReason.User) ScheduleSavePlacement();
        base.OnResized(e);
    }

    private void BaseWindow_PositionChanged(object? sender, PixelPointEventArgs e)
    {
        ScheduleSavePlacement();
    }

    /// <summary>
    /// Saves the placement shortly after the last move or resize, so that dragging the window does not write the settings continuously.
    /// </summary>
    private void ScheduleSavePlacement()
    {
        if (!IsPlaced || string.IsNullOrEmpty(PositionPrefix)) return;

        saveCancellation?.Cancel();
        saveCancellation = new CancellationTokenSource();
        _ = SavePlacementAfterDelay(saveCancellation.Token);
    }

    private async Task SavePlacementAfterDelay(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        var placement = await Dispatcher.InvokeAsync(GetPlacement);
        await SavePlacement(placement);
    }

    private WindowPlacement? GetPlacement()
    {
        if (!IsPlaced || !IsVisible || !CanResize || WindowState != WindowState.Normal) return null;

        return new WindowPlacement(Width, Height, Position.X, Position.Y);
    }

    private async Task SavePlacement(WindowPlacement? placement)
    {
        if (placement == null || string.IsNullOrEmpty(PositionPrefix)) return;

        await SavePlacementSemaphore.WaitAsync();
        try
        {
            // Each setting write notifies all settings listeners, so skip the save when nothing changed.
            if (placement == lastSavedPlacement) return;

            var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
            await settingsService.Set($"{PositionPrefix}_Width", placement.Width);
            await settingsService.Set($"{PositionPrefix}_Height", placement.Height);
            await settingsService.Set($"{PositionPrefix}_PositionX", placement.X);
            await settingsService.Set($"{PositionPrefix}_PositionY", placement.Y);
            lastSavedPlacement = placement;
        }
        catch (Exception e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<BaseWindow>>();
            logger.LogError(e, "[BaseWindow] Error saving the window placement.");
        }
        finally
        {
            SavePlacementSemaphore.Release();
        }
    }

    #endregion
}
