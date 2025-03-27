using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public SidekickViewType ViewType { get; }

    private IServiceScope Scope { get; }

    private bool IsDisposed { get; set; }

    private ICurrentView? View { get; set; }

    public event Action<string>? OnOpenView;

    public MainWindow(SidekickViewType viewType)
    {
        ViewType = viewType;

        Scope = App.ServiceProvider.CreateScope();
        Resources.Add("services", Scope.ServiceProvider);
        InitializeComponent();

        RootComponent.Parameters = new Dictionary<string, object?>
        {
            { "Window", this },
        };

        if (!Debugger.IsAttached)
        {
            WebView.WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            WebView.WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        }

        Topmost = false;
        ShowInTaskbar = false;
        ResizeMode = ResizeMode.NoResize;
        Opacity = 0;

        Show();
    }

    public void OpenView(string url)
    {
        OnOpenView?.Invoke(url);
        Show();
    }

    public void InitializeView(ICurrentView view)
    {
        View = view;
        Dispatcher.InvokeAsync(() =>
        {
            Title = view.Options.Title.StartsWith("Sidekick") ? view.Options.Title.Trim() : $"Sidekick {view.Options.Title}".Trim();

            // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
            // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set opacity. Otherwise, mouse clicks will go through the window.
            WebView.Visibility = Visibility.Visible;
            Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
            Opacity = 0.01;

            switch (ViewType)
            {
                case SidekickViewType.Overlay:
                    Topmost = true;
                    ShowInTaskbar = false;
                    ResizeMode = ResizeMode.CanResize;
                    break;

                case SidekickViewType.Modal:
                    Topmost = false;
                    ShowInTaskbar = true;
                    ResizeMode = ResizeMode.CanResize;
                    break;

                case SidekickViewType.Standard:
                    Topmost = false;
                    ShowInTaskbar = true;
                    ResizeMode = ResizeMode.CanResize;
                    break;
            }

            _ = NormalizeView();
            Activate();
        });
    }

    public void MinimizeView()
    {
        Dispatcher.Invoke(() =>
        {
            WindowState = WindowState.Minimized;
        });
    }

    public void MaximizeView()
    {
        if (WindowState == WindowState.Normal)
        {
            Dispatcher.Invoke(() =>
            {
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
        var viewPreferenceService = Scope.ServiceProvider.GetRequiredService<IViewPreferenceService>();
        var settingsService = Scope.ServiceProvider.GetRequiredService<ISettingsService>();
        var preferences = await viewPreferenceService.Get($"view_preference_{ViewType.ToString()}");
        var saveWindowPositions = await settingsService.GetBool(SettingKeys.SaveWindowPositions);

        Dispatcher.Invoke(() =>
        {
            WindowState = WindowState.Normal;

            MinHeight = View?.Options.Height
                        ?? ViewType switch
                        {
                            SidekickViewType.Modal => 200,
                            _ => 580,
                        };
            MinHeight += 20;
            Height = MinHeight;

            MinWidth = View?.Options.Width
                       ?? ViewType switch
                       {
                           SidekickViewType.Modal => 380,
                           _ => 748,
                       };
            MinWidth += 20;
            Width = MinWidth;

            if (ViewType != SidekickViewType.Modal && preferences != null)
            {
                if (preferences.Height > Height) Height = preferences.Height;
                if (preferences.Width > Width) Width = preferences.Width;
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
        });
    }

    public void CloseView()
    {
        Dispatcher.Invoke(() =>
        {
            WebView.Visibility = Visibility.Hidden;
            Opacity = 0;

            Deactivate();
            Hide();
        });
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
            base.OnClosing(e);
            return;
        }

        base.OnClosing(e);
        return;

        if (!IsVisible || ResizeMode != ResizeMode.CanResize && ResizeMode != ResizeMode.CanResizeWithGrip || WindowState == WindowState.Maximized)
        {
            return;
        }

        // Save the window position and size.
        try
        {
            if (ViewType != SidekickViewType.Modal)
            {
                var width = (int)ActualWidth;
                var height = (int)ActualHeight;
                var x = (int)Left;
                var y = (int)Top;

                var viewPreferenceService = Scope.ServiceProvider.GetRequiredService<IViewPreferenceService>();
                _ = viewPreferenceService.Set(ViewType.ToString(), width, height, x, y);
            }
        }
        catch (Exception)
        {
            // If the save fails, we don't want to stop the execution.
        }
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);

        if (ViewType == SidekickViewType.Overlay)
        {
            _ = Task.Run(async () =>
            {
                var settingsService = Scope.ServiceProvider.GetRequiredService<ISettingsService>();
                var closeOnBlur = await settingsService.GetBool(SettingKeys.OverlayCloseWithMouse);
                if (closeOnBlur)
                {
                    CloseView();
                }
            });
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

    /// <summary>
    /// Temporarily deactivates the current window.
    /// This is typically used to manage focus-related issues when the main window should lose focus.
    /// </summary>
    /// <remarks>
    /// We use this method typically right before we close the window. Sidekick had an issue for the longest time,
    /// where closing the overlay would focus a random window instead of giving the focus back to the game.
    /// This method ensures the window loses focus before closing. The way this is done is by creating a temporary window and giving focus to that.
    /// Windows magic.</remarks>
    public void Deactivate()
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
