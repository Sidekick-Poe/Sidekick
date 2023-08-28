using System.Windows.Controls;
using System.Windows.Interop;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf
{
    internal class OverlayControl : ContentControl, IDisposable
    {
        private OverlayWindow window;
        private HwndHostEx host;

        public OverlayControl()
        {
            Loaded += OverlayControl_Loaded;
        }

        private void OverlayControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            window = new OverlayWindow();
            window.Content = Content;
            window.Show();

            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            Content = new HwndHostEx(windowHandle);
        }

        public void Dispose()
        {
            host?.Dispose();
            window?.Close();
        }
    }
}
