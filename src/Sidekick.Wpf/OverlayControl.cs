using System.Windows.Controls;
using System.Windows.Interop;
using Sidekick.Wpf.Helpers;

namespace Sidekick.Wpf
{
    internal class OverlayControl : ContentControl, IDisposable
    {
        private OverlayWindow? window;

        public OverlayControl()
        {
            Loaded += OverlayControl_Loaded;
        }

        private void OverlayControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            window = new OverlayWindow { Content = Content, };
            window.Show();

            var windowHandle = new WindowInteropHelper(window).Handle;
            Content = new HwndHostEx(windowHandle);
        }

        public void Dispose()
        {
            Loaded -= OverlayControl_Loaded;
            window?.Close();
        }
    }
}
