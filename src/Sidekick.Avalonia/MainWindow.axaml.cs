using System.Globalization;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia;

public partial class MainWindow : Window, IDisposable
{
    // private readonly ILogger logger;
    private readonly IServiceScope scope;
    private bool IsDisposed { get; set; }
    private bool IsNormalized { get; set; }
    private SidekickViewType ViewType { get; }

    public MainWindow(SidekickViewType viewType, string url)
    {
        ViewType = viewType;
        // scope = Program.ServiceProvider.CreateScope();

        Title = "Sidekick";
        InitializeComponent();

        WebView.Navigate(new Uri(url));

        // logger.LogInformation($"[MainWindow] Initialized with view type: {viewType}");
    }

    // public async Task OpenView(string url)
    // {
    //     // logger.LogInformation($"[MainWindow] Opening view: {url}");
//
    //     var fullUrl = url.StartsWith("http") ? url : $"http://localhost:5000{url}";
//
    //     WebView.IsVisible = true;
    //     WebView.Navigate(new Uri(fullUrl));
//
    //     Show();
    //     await NormalizeView();
    //     Activate();
    // }

    public async Task CloseView()
    {
        // logger.LogInformation("[MainWindow] Closing view");
        // await SavePosition();
        IsNormalized = false;
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

    private async Task NormalizeView()
    {
        // Per-type window behavior
        switch (ViewType)
        {
            case SidekickViewType.Overlay:
                Topmost = true;
                ShowInTaskbar = false;
                CanResize = true;
                break;
            case SidekickViewType.Modal:
                Topmost = true;
                ShowInTaskbar = true;
                CanResize = false;
                break;
            case SidekickViewType.Standard:
                Topmost = false;
                ShowInTaskbar = true;
                CanResize = true;
                break;
        }

        // Try to get settings from the Blazor server container (not the host container)
        // var settingsService = Program.ServiceProvider.GetService<ISettingsService>();
        // var viewPreferenceService = Program.ServiceProvider.GetService<IViewPreferenceService>();

        var zoom = 1.0;
        // if (settingsService != null)
        // {
        //     var zoomString = await settingsService.GetString(SettingKeys.Zoom);
        //     if (!double.TryParse(zoomString, CultureInfo.InvariantCulture, out zoom)) zoom = 1;
        // }

        if (IsNormalized && WindowState == WindowState.Normal) return;

        WindowState = WindowState.Normal;

        var minHeight = ViewType switch
        {
            SidekickViewType.Modal => ICurrentView.DialogHeight * zoom,
            SidekickViewType.Standard => 768 * zoom,
            _ => 600 * zoom,
        };
        Height = minHeight;

        var minWidth = ViewType switch
        {
            SidekickViewType.Modal => ICurrentView.DialogWidth * zoom,
            SidekickViewType.Standard => 968 * zoom,
            _ => 768 * zoom,
        };
        Width = minWidth;

        // if (viewPreferenceService != null && ViewType != SidekickViewType.Modal)
        // {
        //     var savePositions = settingsService != null && await settingsService.GetBool(SettingKeys.SaveWindowPositions);
        //     var preferences = await viewPreferenceService.Get(ViewType.ToString());
        //     if (preferences != null)
        //     {
        //         if (preferences.Height > Height) Height = preferences.Height;
        //         if (preferences.Width > Width) Width = preferences.Width;

        //         if (savePositions && preferences.X != null && preferences.Y != null)
        //         {
        //             // Position = new Avalonia.PixelPoint((int)preferences.X.Value, (int)preferences.Y.Value);
        //             IsNormalized = true;
        //             return;
        //         }
        //     }
        // }

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        IsNormalized = true;
    }

    // private async Task SavePosition()
    // {
    //     if (!IsVisible || ViewType == SidekickViewType.Modal || !CanResize || WindowState == WindowState.Maximized) return;

    //     try
    //     {
    //         var viewPreferenceService = Program.ServiceProvider.GetService<IViewPreferenceService>();
    //         if (viewPreferenceService == null) return;

    //         await viewPreferenceService.Set(ViewType.ToString(), (int)Width, (int)Height, Position.X, Position.Y);
    //         logger.LogInformation("[MainWindow] Position saved");
    //     }
    //     catch (Exception e)
    //     {
    //         logger.LogError(e, "[MainWindow] Error saving position");
    //     }
    // }

    public void Dispose()
    {
        if (IsDisposed) return;

        IsDisposed = true;
        scope.Dispose();
        Close();
    }
}

