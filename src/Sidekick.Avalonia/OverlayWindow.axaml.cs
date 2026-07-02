using System.Globalization;
using Avalonia.Controls;
using Avalonia.Media;
using Sidekick.Common.Settings;

namespace Sidekick.Avalonia;

public partial class OverlayWindow : Window, IDisposable
{
    private IServiceProvider ServiceProvider => App.ServerAppHost.Application.Services;
    private const int WIDTH = 768;
    private const int HEIGHT = 600;

    // private readonly ILogger logger;
    private bool IsDisposed { get; set; }

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
    }

    public async Task OpenView(string url)
    {
        WebView.Navigate(new Uri(url));

        if (WindowState != WindowState.Normal) return;

        var settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
        var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out var zoom)) zoom = 1;

        WindowState = WindowState.Normal;
        Height = HEIGHT * zoom;
        Width = WIDTH * zoom;

        // var viewPreferenceService = serviceProvider.GetRequiredService<IViewPreferenceService>();
        // if (viewPreferenceService != null && ViewType != SidekickViewType.Modal)
        // {
        //     var savePositions = settingsService != null && await settingsService.GetBool(SettingKeys.SaveWindowPositions);
        //     var preferences = await viewPreferenceService.Get(ViewType.ToString());
        //     if (preferences != null)
        //     {
        //         if (preferences.Height > Height) Height = preferences.Height;
        //         if (preferences.Width > Width) Width = preferences.Width;
//
        //         if (savePositions && preferences.X != null && preferences.Y != null)
        //         {
        //             // Position = new Avalonia.PixelPoint((int)preferences.X.Value, (int)preferences.Y.Value);
        //             IsNormalized = true;
        //             return;
        //         }
        //     }
        // }

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    public async Task CloseView()
    {
        Hide();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!IsDisposed)
        {
            _ = CloseView();
            e.Cancel = true;
        }

        base.OnClosing(e);
    }

    public void Dispose()
    {
        if (IsDisposed) return;

        IsDisposed = true;
        Close();
    }
}
