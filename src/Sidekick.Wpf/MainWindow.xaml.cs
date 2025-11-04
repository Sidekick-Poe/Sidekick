using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Windows.DllImport;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly ILogger logger;
    private readonly IServiceScope scope;

    private NavigationManager? NavigationManager { get; set; }
    private ICurrentView? View { get; set; }

    private bool IsDisposed { get; set; }
    private bool IsReady { get; set; }
    private bool IsNormalized { get; set; }
    private string? NextPath { get; set; }

    private bool CloseOnBlur { get; set; }
    private IntPtr OriginalFocusedWindow { get; set; }

    private SidekickViewType ViewType { get; }

    public MainWindow(SidekickViewType viewType, ILogger logger)
    {
        this.logger = logger;

        scope = Program.ServiceProvider.CreateScope();
        Resources.Add("services", scope.ServiceProvider);

        Title = "Sidekick";
        ViewType = viewType;
        Width = 0;
        Height = 0;
        Opacity = 0;
        Topmost = false;
        ShowInTaskbar = false;
        InitializeComponent();

        RootComponent.Parameters = new Dictionary<string, object?>
        {
            {
                "Window", this
            },
        };
    }

    public async Task OpenView(string url)
    {
        logger.LogInformation("[MainWindow] Opening view: " + url);

        var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
        CloseOnBlur = await settingsService.GetBool(SettingKeys.OverlayCloseWithMouse);
        OriginalFocusedWindow = User32.GetForegroundWindow();

        await Dispatch(async () =>
        {
            Show();

            if (!IsReady) NextPath = url;
            else Navigate(url);

            Activate();
            await NormalizeView();

            // Attempt to set focus back to the original window
            if (ViewType == SidekickViewType.Overlay && !CloseOnBlur && OriginalFocusedWindow != IntPtr.Zero)
            {
                //User32.SetForegroundWindow(OriginalFocusedWindow);
            }
        });
    }

    public void CloseView()
    {
        logger.LogInformation("[MainWindow] Closing view");

        _ = Dispatch(async () =>
        {
            Navigate("/empty");
            await SavePosition();

            IsNormalized = false;

            Deactivate();
            Hide();
        });
    }

    public void BlazorReady(ICurrentView view, NavigationManager navigationManager)
    {
        logger.LogInformation("[MainWindow] Blazor ready");

        _ = Dispatch(async () =>
        {
            IsReady = true;
            NavigationManager = navigationManager;
            View = view;

            View.OptionsChanged += CurrentViewOptionsChanged;
            View.Maximized += MaximizeView;
            View.Minimized += MinimizeView;
            View.Closed += CloseView;

            Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
            Opacity = 0.01;

            WebView.Visibility = Visibility.Visible;
            SetWebViewDebugging();

            await NormalizeView();
            Navigate(NextPath);
        });
    }

    private void CurrentViewOptionsChanged()
    {
        _ = Dispatch(NormalizeView);
    }

    private void MinimizeView()
    {
        logger.LogInformation("[MainWindow] Minimizing view");

        _ = Dispatch(async () =>
        {
            await SavePosition();
            WindowState = WindowState.Minimized;
        });
    }

    private void MaximizeView()
    {
        logger.LogInformation("[MainWindow] Maximizing view");

        _ = Dispatch(async () =>
        {
            if (WindowState == WindowState.Normal)
            {
                await SavePosition();
                WindowState = WindowState.Maximized;
            }
            else
            {
                await NormalizeView();
            }
        });
    }

    private async Task NormalizeView()
    {
        logger.LogInformation("[MainWindow] Normalizing view");

        switch (ViewType)
        {
            case SidekickViewType.Overlay:
#if DEBUG
                Topmost = false;
                ShowInTaskbar = true;
#else
                Topmost = true;
                ShowInTaskbar = false;
#endif
                ResizeMode = ResizeMode.CanResize;
                break;

            case SidekickViewType.Modal:
                Topmost = true;
                ShowInTaskbar = true;
                ResizeMode = ResizeMode.NoResize;
                break;

            case SidekickViewType.Standard:
                Topmost = false;
                ShowInTaskbar = true;
                ResizeMode = ResizeMode.CanResize;
                break;
        }

        var viewPreferenceService = scope.ServiceProvider.GetRequiredService<IViewPreferenceService>();
        var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
        var preferences = await viewPreferenceService.Get(ViewType.ToString());
        var saveWindowPositions = await settingsService.GetBool(SettingKeys.SaveWindowPositions);
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        if (View is
            {
                Height: not null,
                Width: not null,
            })
        {
            logger.LogInformation("[MainWindow] View has fixed dimensions");

            WindowState = WindowState.Normal;
            ResizeMode = ResizeMode.NoResize;

            MinHeight = (View.Height.Value + 20) * zoom;
            Height = MinHeight;

            MinWidth = (View.Width.Value + 20) * zoom;
            Width = MinWidth;

            CenterHelper.Center(this);

            IsNormalized = false;
            return;
        }

        if (IsNormalized && WindowState == WindowState.Normal)
        {
            logger.LogInformation("[MainWindow] View is already normalized");
            return;
        }

        logger.LogInformation("[MainWindow] View is not normalized");
        WindowState = WindowState.Normal;

        MinHeight = ViewType switch
        {
            SidekickViewType.Modal => ICurrentView.DialogHeight * zoom,
            SidekickViewType.Standard => 768 * zoom,
            _ => 600 * zoom,
        };
        Height = MinHeight;

        MinWidth = ViewType switch
        {
            SidekickViewType.Modal => ICurrentView.DialogWidth * zoom,
            SidekickViewType.Standard => 968 * zoom,
            _ => 768 * zoom,
        };
        Width = MinWidth;

        if (ViewType != SidekickViewType.Modal && preferences != null)
        {
            if (preferences.Height > Height && View?.Height == null) Height = preferences.Height;
            if (preferences.Width > Width && View?.Width == null) Width = preferences.Width;
        }

        // Set the window position.
        if (saveWindowPositions
            && preferences is
            {
                X: not null,
                Y: not null,
            })
        {
            Left = preferences.X.Value;
            Top = preferences.Y.Value;
        }
        else
        {
            CenterHelper.Center(this);
        }

        IsNormalized = true;
    }

    private void Navigate(string? url)
    {
        if (NavigationManager == null || !IsReady || string.IsNullOrEmpty(url)) return;

        NavigationManager.NavigateTo(url);
    }

    private async Task SavePosition()
    {
        if (!IsVisible || ViewType == SidekickViewType.Modal || ResizeMode is not (ResizeMode.CanResize or ResizeMode.CanResizeWithGrip) || WindowState == WindowState.Maximized)
        {
            logger.LogInformation("[MainWindow] Not saving position, window is not visible, is modal, or is maximized");
            return;
        }

        try
        {
            var width = (int)ActualWidth;
            var height = (int)ActualHeight;
            var x = (int)Left;
            var y = (int)Top;

            var viewPreferenceService = scope.ServiceProvider.GetRequiredService<IViewPreferenceService>();
            await viewPreferenceService.Set(ViewType.ToString(), width, height, x, y);
            logger.LogInformation("[MainWindow] Position saved");
        }
        catch (Exception e)
        {
            logger.LogError(e, "[MainWindow] Error saving position");
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!IsDisposed)
        {
            CloseView();
            e.Cancel = true;
        }

        base.OnClosing(e);
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);

        if (ViewType == SidekickViewType.Overlay && CloseOnBlur)
        {
            CloseView();
        }
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        Grid.Margin = WindowState == WindowState.Maximized ? new Thickness(0) : new Thickness(5);
    }

    private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void SetWebViewDebugging()
    {
#if DEBUG
        return;
#endif

        WebView.WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        WebView.WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        WebView.WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
    }

    /// <summary>
    /// Temporarily deactivates the current window.
    /// This is typically used to manage focus-related issues when the main window should lose focus.
    /// </summary>
    /// <remarks>
    /// We use this method typically right before we close the window. Sidekick had an issue for the longest time,
    /// where closing the overlay would focus a random window instead of giving the focus back to the game.
    /// This method ensures the window loses focus before closing. The way this is done is by creating a temporary window and giving focus to that.
    /// Windows magic.</remarks>
    private void Deactivate()
    {
        // Check if the window is still valid and focused
        if (!IsActive) return;

        // Create a hidden fake window to take focus temporarily
        var helperWindow = new Window
        {
            Width = 1,
            Height = 1,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = null,
            Opacity = 0
        };

        // Show the helper window to take focus
        helperWindow.Show();

        // Set focus to the fake window before closing it
        helperWindow.Activate();
        helperWindow.Close();
    }

    private async Task Dispatch(Func<Task> action)
    {
        await Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                logger.LogError(e, "[MainWindow] Error dispatching");
            }
        });
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        MaximizeHelper.AddHook(this);
    }

    public void Dispose()
    {
        IsDisposed = true;
        MaximizeHelper.RemoveHook(this);
        Resources.Remove("services");
        if (View != null)
        {
            View.OptionsChanged -= CurrentViewOptionsChanged;
            View.Maximized -= MaximizeView;
            View.Minimized -= MinimizeView;
            View.Closed -= CloseView;
        }

        scope.Dispose();
        Close();
    }
}
