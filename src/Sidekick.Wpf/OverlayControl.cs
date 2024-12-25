using System.Windows.Controls;
using System.Windows.Interop;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf;

internal class OverlayControl : ContentControl, IDisposable
{
    private OverlayWindow? window;

    public OverlayControl()
    {
        Loaded += OverlayControl_Loaded;
        Unloaded += OverlayControl_Unloaded;
    }

    private void OverlayControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        window = new OverlayWindow { Content = Content, };
        window.Show();

        var windowHandle = new WindowInteropHelper(window).Handle;
        Content = new HwndHostEx(windowHandle);
    }

    private void OverlayControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        Dispose(); // Call cleanup in Unloaded
    }

    public void Dispose()
    {
        // Unsubscribe from events
        Loaded -= OverlayControl_Loaded;
        Unloaded -= OverlayControl_Unloaded; // Unsubscribe here as well

        // Dispose of any resources
        window?.Close();
    }
}
