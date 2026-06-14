using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sidekick.Wpf;

public partial class TransparentFocusWindow : Window
{
    private readonly TaskCompletionSource focusTakenCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool isClosing;

    public TransparentFocusWindow()
    {
        InitializeComponent();
    }

    public Task FocusTakenTask => focusTakenCompletionSource.Task;

    private void TransparentFocusWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Width = 1;
        Height = 1;

        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
    }

    private void TransparentFocusWindow_ContentRendered(object? sender, EventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            Topmost = false;
            Topmost = true;

            Activate();
            Focus();

            Dispatcher.BeginInvoke(() =>
            {
                if (!IsActive)
                {
                    Activate();
                    Focus();
                }

                focusTakenCompletionSource.TrySetResult();
            }, DispatcherPriority.ApplicationIdle);
        }, DispatcherPriority.ApplicationIdle);
    }

    private void TryClose()
    {
        if (isClosing) return;

        isClosing = true;
        Close();
    }

    private void TransparentFocusWindow_Deactivated(object? sender, EventArgs e)
    {
        TryClose();
    }

    private void TransparentFocusWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        TryClose();
    }

    private void TransparentFocusWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        TryClose();
    }

    private void TransparentFocusWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) TryClose();
    }
}
