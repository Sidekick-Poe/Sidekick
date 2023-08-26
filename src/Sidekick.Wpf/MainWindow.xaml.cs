using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IServiceScope Scope { get; set; }

    public MainWindow()
    {
        Scope = App.ServiceProvider.CreateScope();

        Resources.Add("services", Scope.ServiceProvider);
        InitializeComponent();
    }

    public string CurrentWebPath
    {
        get
        {
            return WebView.WebView.Source.ToString();
        }
    }

    public void Ready()
    {
        // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
        WebView.Visibility = Visibility.Visible;

        // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set a background color. Otherwise mouse clicks will go through the window.
        Background = (Brush?)new BrushConverter().ConvertFrom("#000000");

        InvalidateVisual();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Scope.Dispose();
    }
}
