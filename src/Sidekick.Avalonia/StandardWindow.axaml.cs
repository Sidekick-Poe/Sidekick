using System.Globalization;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Sidekick.Common.Helpers;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class StandardWindow : Window
{
    private const int WIDTH = 968;
    private const int HEIGHT = 768;
    private const string POSITION_PREFIX = "StandardWindow";

    private IServiceProvider ServiceProvider => App.RequiredServerAppHost.Application.Services;

    private bool IsReady { get; set; }

    public StandardWindow()
    {
        Title = "Sidekick";
        Width = WIDTH;
        Height = HEIGHT;
        MinWidth = WIDTH;
        MinHeight = HEIGHT;
        Background = Brushes.Transparent;
        WindowDecorations = WindowDecorations.None;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = false;
        ShowInTaskbar = true;
        CanResize = true;

        InitializeComponent();
    }

    public async Task OpenView(string url)
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            if (IsVisible)
            {
                _ = WebView.InvokeScript($"sidekick.app.navigationManager.navigateTo({JsonSerializer.Serialize(url)});");
                Activate();
            }
            else
            {
                Show();
                WebView.Navigate(new Uri(url));
                await NormalizeView();
            }

            IsReady = true;
        });
    }

    private async Task NormalizeView()
    {
        WindowState = WindowState.Normal;
        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        Height = HEIGHT * zoom;
        Width = WIDTH * zoom;
        MinWidth = Width;
        MinHeight = Height;

        if (await settingsService.GetBool(SettingKeys.SaveWindowPositions))
        {
            var positionX = await settingsService.GetInt($"{POSITION_PREFIX}_PositionX");
            var positionY = await settingsService.GetInt($"{POSITION_PREFIX}_PositionY");
            Position = new PixelPoint(positionX, positionY);
            var height = await settingsService.GetDouble($"{POSITION_PREFIX}_Height");
            var width = await settingsService.GetDouble($"{POSITION_PREFIX}_Width");
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

    public void MinimizeView()
    {
        Dispatcher.InvokeAsync(async () =>
        {
            await SavePosition();
            WindowState = WindowState.Minimized;
        });
    }

    public void MaximizeView()
    {
        Dispatcher.InvokeAsync(async () =>
        {
            if (WindowState == WindowState.Normal)
            {
                await SavePosition();
                WindowState = WindowState.Maximized;
                WebView.Margin = new Thickness(0);
            }
            else
            {
                await NormalizeView();
                WebView.Margin = new Thickness(5);
            }
        });
    }

    #region Save Position

    private static readonly SemaphoreSlim SavePositionSemaphore = new(1, 1);

    public async Task SavePosition()
    {
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
            await settingsService.Set($"{POSITION_PREFIX}_Width", position.Width);
            await settingsService.Set($"{POSITION_PREFIX}_Height", position.Height);
            await settingsService.Set($"{POSITION_PREFIX}_PositionX", position.X);
            await settingsService.Set($"{POSITION_PREFIX}_PositionY", position.Y);
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

    #region Resize

    private void TopResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.North, e);
    }

    private void BottomResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.South, e);
    }

    private void LeftResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.West, e);
    }

    private void RightResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.East, e);
    }

    private void TopLeftResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.NorthWest, e);
    }

    private void TopRightResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.NorthEast, e);
    }

    private void BottomLeftResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.SouthWest, e);
    }

    private void BottomRightResizeHandle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResize(WindowEdge.SouthEast, e);
    }

    private void BeginResize(WindowEdge edge, PointerPressedEventArgs e)
    {
        if (!CanResize || WindowState == WindowState.Maximized)
        {
            return;
        }

        BeginResizeDrag(edge, e);
    }

    private Debouncer ResizeSavePositionDebouncer { get; } = new();

    protected override void OnResized(WindowResizedEventArgs e)
    {
        base.OnResized(e);
        if (!IsVisible || !CanResize || !IsReady) return;
        ResizeSavePositionDebouncer.Debounce(() =>
        {
            _ = SavePosition();
        }, 100);
    }

    #endregion Resize
}
