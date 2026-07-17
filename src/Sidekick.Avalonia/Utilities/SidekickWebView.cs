using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Sidekick.Avalonia.Utilities;

public class SidekickWebView : NativeWebView
{
    public SidekickWebView()
    {
        // Force the native window container to occupy 0 space initially
        Width = 0;
        Height = 0;
        
        EnvironmentRequested += OnEnvironmentRequested;
        NavigationCompleted += OnNavigationCompleted;
    }
    
    private void OnEnvironmentRequested(object? sender, WebViewEnvironmentRequestedEventArgs args)
    {
#if DEBUG
        // Enable developer tools for all platforms
        args.EnableDevTools = true;
#endif
        
        if (args is LinuxWpeWebViewEnvironmentRequestedEventArgs wpeArgs)
        {
            wpeArgs.PreferWebKitGtkInstead = true;
        }

        if (args is WindowsWebView2EnvironmentRequestedEventArgs webView2Args)
        {
            webView2Args.IsInPrivateModeEnabled = true;
        }
        
        if (args is AppleWKWebViewEnvironmentRequestedEventArgs appleArgs)
        {
            appleArgs.NonPersistentDataStore = true;
        }
        
        if (args is GtkWebViewEnvironmentRequestedEventArgs gtkArgs)
        {
            gtkArgs.EphemeralDataManager = true;
        }
    }
    
    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        // The DOM is parsed and the initial background/CSS has rendered. 
        // Reveal the control on the UI thread safely.
        Dispatcher.UIThread.Post(() =>
        {
            Width = double.NaN;
            Height = double.NaN;
            
            // Trigger layout recalculation 
            InvalidateMeasure();
            
            // Execute the native Linux resize sync hack
            var currentVisibility = IsVisible;
            IsVisible = false;
            IsVisible = currentVisibility;
        }, DispatcherPriority.Render);
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Force a tiny layout tick on the next frame so Avalonia re-evaluates the bounds
        Dispatcher.Post(() =>
        {
            var currentVisibility = IsVisible;
            IsVisible = false;
            IsVisible = currentVisibility;
        
            // Explicitly force a measure pass
            InvalidateMeasure();
        }, DispatcherPriority.Background);
    }
}