using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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

    public SidekickViewType ViewType { get; }

    private IServiceScope Scope { get; }

    private bool IsDisposed { get; set; }

    public ICurrentView? View { get; private set; }

    public event Action? ViewOpened;

    public string? Url { get; private set; }

    private bool ViewNormalized { get; set; }

    private bool CloseOnBlur { get; set; }

    private IntPtr OriginalFocusedWindow { get; set; }

    public MainWindow(SidekickViewType viewType, ILogger logger)
    {
        this.logger = logger;
        ViewType = viewType;

        Scope = Program.ServiceProvider.CreateScope();
        Resources.Add("services", Scope.ServiceProvider);
        InitializeComponent();

        RootComponent.Parameters = new Dictionary<string, object?>
        {
            { "Window", this },
        };

        Show();
    }

    public async Task OpenView(string url)
    {
        var settingsService = Scope.ServiceProvider.GetRequiredService<ISettingsService>();
        CloseOnBlur = await settingsService.GetBool(SettingKeys.OverlayCloseWithMouse);

        OriginalFocusedWindow = User32.GetForegroundWindow();

        logger.LogInformation("[MainWindow] Opening view: " + url);

        Url = url;
        ViewOpened?.Invoke();
        Dispatcher.InvokeAsync(Show);
    }

    public void InitializeView(ICurrentView view)
    {
        logger.LogInformation("[MainWindow] Initializing view: " + view.Options.Title);

        View = view;
        Dispatcher.InvokeAsync(() =>
        {
            Title = view.Options.Title.StartsWith("Sidekick") ? view.Options.Title.Trim() : $"Sidekick {view.Options.Title}".Trim();

            // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
            // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set opacity. Otherwise, mouse clicks will go through the window.
            WebView.Visibility = Visibility.Visible;
            SetWebViewDebugging();

            Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
            Opacity = 0.01;

            switch (ViewType)
            {
                case SidekickViewType.Overlay:
                    Topmost = false;
                    Topmost = true;
                    ShowInTaskbar = false;
                    ResizeMode = ResizeMode.CanResize;
                    break;

                case SidekickViewType.Modal:
                    Topmost = false;
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

            if (view.Options is
                {
                    Width: not null,
                    Height: not null
                })
            {
                ResizeMode = ResizeMode.NoResize;
            }

            if (WindowState == WindowState.Normal)
            {
                _ = NormalizeView();
            }

            Activate();

            // Attempt to set focus back to the original window
            if (ViewType == SidekickViewType.Overlay && !CloseOnBlur && OriginalFocusedWindow != IntPtr.Zero)
            {
                User32.SetForegroundWindow(OriginalFocusedWindow);
            }
        });
    }

    public void MinimizeView()
    {
        logger.LogInformation("[MainWindow] Minimizing view");

        Dispatcher.InvokeAsync(async () =>
        {
            await SavePosition();
            WindowState = WindowState.Minimized;
        });
    }

    public void MaximizeView()
    {
        logger.LogInformation("[MainWindow] Maximizing view");

        if (WindowState == WindowState.Normal)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await SavePosition();
                WindowState = WindowState.Maximized;
            });
        }
        else
        {
            _ = NormalizeView();
        }
    }

    public async Task NormalizeView()
    {
        logger.LogInformation("[MainWindow] Normalizing view");

        var viewPreferenceService = Scope.ServiceProvider.GetRequiredService<IViewPreferenceService>();
        var settingsService = Scope.ServiceProvider.GetRequiredService<ISettingsService>();
        var preferences = await viewPreferenceService.Get(ViewType.ToString());
        var saveWindowPositions = await settingsService.GetBool(SettingKeys.SaveWindowPositions);
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        Dispatcher.Invoke(() =>
        {
            if (View is
                {
                    Options:
                    {
                        Height: not null,
                        Width: not null,
                    }
                })
            {
                logger.LogInformation("[MainWindow] View has fixed dimensions");

                WindowState = WindowState.Normal;

                MinHeight = (View.Options.Height.Value + 20) * zoom;
                Height = MinHeight;

                MinWidth = (View.Options.Width.Value + 20) * zoom;
                Width = MinWidth;

                CenterHelper.Center(this);

                ViewNormalized = false;
                return;
            }

            if (ViewNormalized && WindowState == WindowState.Normal)
            {
                logger.LogInformation("[MainWindow] View is already normalized");
                return;
            }

            logger.LogInformation("[MainWindow] View is not normalized");
            WindowState = WindowState.Normal;

            MinHeight = ViewType switch
            {
                SidekickViewType.Modal => 220 * zoom,
                _ => 600 * zoom,
            };
            Height = MinHeight;

            MinWidth = ViewType switch
            {
                SidekickViewType.Modal => 400 * zoom,
                _ => 768 * zoom,
            };
            Width = MinWidth;

            if (ViewType != SidekickViewType.Modal && preferences != null)
            {
                if (preferences.Height > Height && View?.Options!.Height == null) Height = preferences.Height;
                if (preferences.Width > Width && View?.Options!.Width == null) Width = preferences.Width;
            }

            // Set the window position.
            if (saveWindowPositions
                && preferences is
                {
                    X: not null,
                    Y: not null
                })
            {
                Left = preferences.X.Value;
                Top = preferences.Y.Value;
            }
            else
            {
                CenterHelper.Center(this);
            }

            ViewNormalized = true;
        });
    }

    public void CloseView()
    {
        logger.LogInformation("[MainWindow] Closing view");

        Dispatcher.InvokeAsync(async () =>
        {
            await SavePosition();
            ViewNormalized = false;

            Topmost = false;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;

            WebView.Visibility = Visibility.Hidden;

            Deactivate2();
            Hide();
        });
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

            var viewPreferenceService = Scope.ServiceProvider.GetRequiredService<IViewPreferenceService>();
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
            if (View != null)
                View.Close();
            else
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
        if (Debugger.IsAttached) return;

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
    private void Deactivate2()
    {
        // Check if the window is still valid and focused
        if (!IsActive)
        {
            return;
        }

        // Create a hidden dummy window to take focus temporarily
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

        // Set focus to the dummy window before closing it
        helperWindow.Activate();
        helperWindow.Close();
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
        Scope.Dispose();
        Close();
    }
}
