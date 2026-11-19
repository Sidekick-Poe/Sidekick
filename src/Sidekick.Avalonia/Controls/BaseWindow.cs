using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Sidekick.Common.Cache;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

using Win32Interop = Sidekick.Avalonia.Utilities.WindowsNativeMethods;

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
        Background = SolidColorBrush.Parse("#1C1917");
        Opened += BaseWindow_Opened;
        PositionChanged += BaseWindow_PositionChanged;
        Deactivated += BaseWindow_Deactivated;
    }

    /// <summary>
    /// A hotkey being recorded in the settings stops when the window loses focus, so that a key pressed in the game is not recorded.
    /// The web view can't detect this on Linux, since it does not fire blur events.
    /// </summary>
    private void BaseWindow_Deactivated(object? sender, EventArgs e)
    {
        var input = ServiceProvider.GetService<IInputProvider>();
        if (input != null) input.IsRecordingKeybind = false;
    }

    private void BaseWindow_Opened(object? sender, EventArgs e)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var platformHandle = TryGetPlatformHandle();
        if (platformHandle == null)
        {
            return;
        }

        var hWnd = platformHandle.Handle;
        var useDarkMode = 1;
        Win32Interop.DwmSetWindowAttribute(hWnd, Win32Interop.DwmwaUseImmersiveDarkMode, ref useDarkMode, sizeof(int));

        var borderColor = Win32Interop.DwmwaColorNone;
        Win32Interop.DwmSetWindowAttribute(hWnd, Win32Interop.DwmwaBorderColor, ref borderColor, sizeof(int));

        // Colors are 0x00BBGGRR. Windows 10 ignores them and keeps its dark gray title bar.
        var captionColor = 0x00090A0C;
        Win32Interop.DwmSetWindowAttribute(hWnd, Win32Interop.DwmwaCaptionColor, ref captionColor, sizeof(int));

        var textColor = 0x00FFFFFF;
        Win32Interop.DwmSetWindowAttribute(hWnd, Win32Interop.DwmwaTextColor, ref textColor, sizeof(int));
    }

    public void CloseView()
    {
        var scopedCache = ServiceProvider.GetRequiredService<IScopedCacheProvider>();
        scopedCache.Clear();

        Dispatcher.Invoke(() =>
        {
            // Save the final placement now, since a hidden window cannot be measured anymore.
            var placement = GetPlacement();
            saveCancellation?.Cancel();
            Hide();
            IsPlaced = false;

            // The page that requested a size is no longer shown; the next page requests its own size.
            RequestedSizeOwner = null;
            RequestedWidth = null;
            RequestedHeight = null;
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
        var zoom = await GetZoom();

        var minWidth = (RequestedWidth ?? DefaultWidth) * zoom;
        var minHeight = (RequestedHeight ?? DefaultHeight) * zoom;
        var width = minWidth;
        var height = minHeight;

        PixelPoint? savedPosition = null;
        if (!string.IsNullOrEmpty(PositionPrefix))
        {
            // The size is always remembered. The setting only controls the position.
            var savedWidth = await settingsService.GetDoubleAsync($"{PositionPrefix}_Width");
            var savedHeight = await settingsService.GetDoubleAsync($"{PositionPrefix}_Height");
            if (savedWidth >= width && savedHeight >= height)
            {
                width = savedWidth;
                height = savedHeight;
            }

            // A saved width of 0 means that no placement was saved yet.
            if (savedWidth > 0 && await settingsService.GetBoolAsync(SettingKeys.SaveWindowPositions))
            {
                var positionX = await settingsService.GetIntAsync($"{PositionPrefix}_PositionX");
                var positionY = await settingsService.GetIntAsync($"{PositionPrefix}_PositionY");
                savedPosition = new PixelPoint(positionX, positionY);
            }
        }

        var screen = (savedPosition != null ? Screens.ScreenFromPoint(savedPosition.Value) : null)
                     ?? Screens.ScreenFromWindow(this)
                     ?? Screens.Primary;
        if (screen != null)
        {
            (width, height) = FitToScreen(screen, width, height);
            (minWidth, minHeight) = FitToScreen(screen, minWidth, minHeight);
        }

        // The saved size is the user's choice, but they can still shrink the window back to the default or requested size.
        ApplySize(width, height, minWidth, minHeight);

        if (screen != null)
        {
            var (pixelWidth, pixelHeight) = ToPixels(screen, width, height);
            var workingArea = screen.WorkingArea;
            var position = savedPosition ?? new PixelPoint(
                workingArea.X + (workingArea.Width - pixelWidth) / 2,
                workingArea.Y + (workingArea.Height - pixelHeight) / 2);
            Position = ClampToScreen(screen, position, pixelWidth, pixelHeight);
        }

        // Remember the placement we just applied, so that it is not saved again unless the user changes it.
        lastSavedPlacement = new WindowPlacement(Width, Height, Position.X, Position.Y);
        IsPlaced = true;
    }

    #region Requested Size

    private int? RequestedWidth { get; set; }
    private int? RequestedHeight { get; set; }

    /// <summary>
    /// The page that requested the current size. A page that is no longer shown cannot reset the size requested by another one.
    /// </summary>
    private object? RequestedSizeOwner { get; set; }

    public Task SetRequestedSize(int width, int height, object owner)
    {
        RequestedSizeOwner = owner;
        return ApplyRequestedSize(width, height);
    }

    public Task ResetRequestedSize(object owner)
    {
        if (!ReferenceEquals(RequestedSizeOwner, owner)) return Task.CompletedTask;

        RequestedSizeOwner = null;
        return ApplyRequestedSize(null, null);
    }

    /// <summary>
    /// Applies the requested size, or the default when null.
    /// An open window is resized around its current center, a hidden window uses the size the next time it opens.
    /// </summary>
    private async Task ApplyRequestedSize(int? width, int? height)
    {
        if (RequestedWidth == width && RequestedHeight == height) return;

        RequestedWidth = width;
        RequestedHeight = height;
        if (!IsPlaced || !IsVisible) return;

        var zoom = await GetZoom();
        var screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;
        if (screen == null) return;

        // Keep the window centered on where it currently is, so that a window the user moved does not jump.
        var (currentPixelWidth, currentPixelHeight) = ToPixels(screen, Width, Height);
        var centerX = Position.X + currentPixelWidth / 2;
        var centerY = Position.Y + currentPixelHeight / 2;

        var (newWidth, newHeight) = FitToScreen(screen, (width ?? DefaultWidth) * zoom, (height ?? DefaultHeight) * zoom);
        ApplySize(newWidth, newHeight, newWidth, newHeight);

        var (pixelWidth, pixelHeight) = ToPixels(screen, newWidth, newHeight);
        Position = ClampToScreen(screen, new PixelPoint(centerX - pixelWidth / 2, centerY - pixelHeight / 2), pixelWidth, pixelHeight);

        // A size requested by a page is not the user's choice, so it is not saved.
        lastSavedPlacement = new WindowPlacement(Width, Height, Position.X, Position.Y);
    }

    #endregion

    #region Screen Helpers

    private async Task<double> GetZoom()
    {
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetStringAsync(SettingKeys.Zoom);
        return double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom) ? zoom : 1;
    }

    private void ApplySize(double width, double height, double minWidth, double minHeight)
    {
        // The minimum is set first, so that the window can both grow and shrink.
        MinWidth = minWidth;
        MinHeight = minHeight;
        Width = width;
        Height = height;
    }

    private static (double Width, double Height) FitToScreen(Screen screen, double width, double height)
    {
        return (Math.Min(width, screen.WorkingArea.Width / screen.Scaling),
                Math.Min(height, screen.WorkingArea.Height / screen.Scaling));
    }

    private static (int Width, int Height) ToPixels(Screen screen, double width, double height)
    {
        return ((int)Math.Ceiling(width * screen.Scaling), (int)Math.Ceiling(height * screen.Scaling));
    }

    private static PixelPoint ClampToScreen(Screen screen, PixelPoint position, int pixelWidth, int pixelHeight)
    {
        var workingArea = screen.WorkingArea;
        var maxX = Math.Max(workingArea.X, workingArea.Right - pixelWidth);
        var maxY = Math.Max(workingArea.Y, workingArea.Bottom - pixelHeight);
        return new PixelPoint(
            Math.Clamp(position.X, workingArea.X, maxX),
            Math.Clamp(position.Y, workingArea.Y, maxY));
    }

    #endregion

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
            await settingsService.SetAsync($"{PositionPrefix}_Width", placement.Width);
            await settingsService.SetAsync($"{PositionPrefix}_Height", placement.Height);
            await settingsService.SetAsync($"{PositionPrefix}_PositionX", placement.X);
            await settingsService.SetAsync($"{PositionPrefix}_PositionY", placement.Y);
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
