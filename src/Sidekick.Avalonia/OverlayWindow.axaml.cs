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

public partial class OverlayWindow : Window
{
    private const int WIDTH = 768;
    private const int HEIGHT = 600;
    private const string POSITION_PREFIX = "OverlayWindow";

    private IServiceProvider ServiceProvider => App.RequiredServerAppHost.Application.Services;

    private bool IsReady { get; set; }

    public OverlayWindow()
    {
        Title = "Sidekick";
        Width = WIDTH;
        Height = HEIGHT;
        Background = Brushes.Transparent;
        WindowDecorations = WindowDecorations.None;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = true;
        ShowInTaskbar = false;
        CanResize = true;

        InitializeComponent();

        WebView.EnvironmentRequested += (_, args) =>
        {
#if DEBUG
            // Enable developer tools for all platforms
            args.EnableDevTools = true;
#endif

            // Platform-specific configuration
            switch (args)
            {
                case WindowsWebView2EnvironmentRequestedEventArgs webView2Args:
                    // This causes the application to crash. We need investigation.
                    // var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
                    // ar useHardwareAcceleration = settingsService.GetBool(SettingKeys.UseHardwareAcceleration).Result;
                    // if (!useHardwareAcceleration) webView2Args.AdditionalBrowserArguments = "--disable-gpu";
                    webView2Args.IsInPrivateModeEnabled = true;
                    break;
                case AppleWKWebViewEnvironmentRequestedEventArgs appleArgs:
                    appleArgs.NonPersistentDataStore = true;
                    break;
                case GtkWebViewEnvironmentRequestedEventArgs gtkArgs:
                    gtkArgs.EphemeralDataManager = true;
                    break;
            }
        };
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
            }

            await NormalizeView();
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

    private async Task SavePosition()
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            if (!IsVisible || !CanResize || WindowState == WindowState.Maximized) return;

            try
            {
                var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
                await settingsService.Set($"{POSITION_PREFIX}_Width", Width);
                await settingsService.Set($"{POSITION_PREFIX}_Height", Height);
                await settingsService.Set($"{POSITION_PREFIX}_PositionX", Position.X);
                await settingsService.Set($"{POSITION_PREFIX}_PositionY", Position.Y);
            }
            catch (Exception e)
            {
                var logger = ServiceProvider.GetRequiredService<ILogger<StandardWindow>>();
                logger.LogError(e, "[MainWindow] Error saving position");
            }
        });
    }

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
