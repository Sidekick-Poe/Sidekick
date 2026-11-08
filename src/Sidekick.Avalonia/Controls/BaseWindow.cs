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
    private bool IsReady { get; set; }

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
        Dispatcher.Invoke(Hide);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        CloseView();
        e.Cancel = true;
        base.OnClosing(e);
    }

    protected async Task NormalizeView()
    {
        WindowState = WindowState.Normal;
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        Height = DefaultHeight * zoom;
        Width = DefaultWidth * zoom;
        MinWidth = Width;
        MinHeight = Height;

        if (!string.IsNullOrEmpty(PositionPrefix))
        {
            if (await settingsService.GetBool(SettingKeys.SaveWindowPositions))
            {
                var positionX = await settingsService.GetInt($"{PositionPrefix}_PositionX");
                var positionY = await settingsService.GetInt($"{PositionPrefix}_PositionY");
                Position = new PixelPoint(positionX, positionY);
            }

            var height = await settingsService.GetDouble($"{PositionPrefix}_Height");
            var width = await settingsService.GetDouble($"{PositionPrefix}_Width");
            if (height >= Height && width >= Width)
            {
                Height = height;
                Width = width;
            }
        }

        // EnsurePositionWithinScreenBounds
        var screen = Screens.ScreenFromPoint(Position) ?? Screens.Primary;
        if (screen == null)
        {
            return;
        }

        var workingArea = screen.WorkingArea;
        var maxX = Math.Max(workingArea.X, workingArea.Right - (int)Width);
        var maxY = Math.Max(workingArea.Y, workingArea.Bottom - (int)Height);

        var x = Math.Clamp(Position.X, workingArea.X, maxX);
        var y = Math.Clamp(Position.Y, workingArea.Y, maxY);

        Position = new PixelPoint(x, y);
        IsReady = true;
    }

    #region Save Position

    private static readonly SemaphoreSlim SavePositionSemaphore = new(1, 1);

    protected override void OnResized(WindowResizedEventArgs e)
    {
        Dispatcher.Invoke(SavePosition);
        base.OnResized(e);
    }

    private async Task SavePosition()
    {
        if (!IsReady || string.IsNullOrEmpty(PositionPrefix)) return;
        if (!await SavePositionSemaphore.WaitAsync(0)) return;

        try
        {
            var position = await Dispatcher.InvokeAsync(() =>
            {
                if (!IsVisible || !CanResize || WindowState == WindowState.Maximized)
                {
                    return null;
                }

                return new
                {
                    Width,
                    Height,
                    Position.X,
                    Position.Y,
                };
            });

            if (position == null)
            {
                return;
            }

            var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
            await settingsService.Set($"{PositionPrefix}_Width", position.Width);
            await settingsService.Set($"{PositionPrefix}_Height", position.Height);
            await settingsService.Set($"{PositionPrefix}_PositionX", position.X);
            await settingsService.Set($"{PositionPrefix}_PositionY", position.Y);
        }
        catch (Exception e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<StandardWindow>>();
            logger.LogError(e, "[StandardWindow] Error saving position");
        }
        finally
        {
            SavePositionSemaphore.Release();
        }
    }

    #endregion
}
