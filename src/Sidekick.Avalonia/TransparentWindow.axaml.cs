using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace Sidekick.Avalonia;

public partial class TransparentWindow : Window
{
    private readonly TaskCompletionSource focusTakenCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool isClosing;

    public TransparentWindow()
    {
        Title = "Sidekick";
        Width = 1;
        Height = 1;
        MinWidth = 1;
        MinHeight = 1;
        MaxWidth = 1;
        MaxHeight = 1;

        WindowDecorations = WindowDecorations.None;
        WindowStartupLocation = WindowStartupLocation.Manual;
        ShowInTaskbar = false;
        Topmost = true;
        CanResize = false;
        Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
        TransparencyLevelHint = [WindowTransparencyLevel.Transparent];

        InitializeComponent();

        Opened += TransparentWindow_Opened;
        Deactivated += TransparentWindow_Deactivated;
        PointerPressed += TransparentWindow_PointerPressed;
        PointerReleased += TransparentWindow_PointerReleased;
        KeyDown += TransparentWindow_KeyDown;
    }

    public Task FocusTakenTask => focusTakenCompletionSource.Task;

    private void TransparentWindow_Opened(object? sender, EventArgs e)
    {
        MoveToVirtualScreenOrigin();

        Dispatcher.UIThread.Post(() =>
        {
            Topmost = false;
            Topmost = true;

            Activate();
            Focus();

            Dispatcher.UIThread.Post(() =>
            {
                if (!IsActive)
                {
                    Activate();
                    Focus();
                }

                focusTakenCompletionSource.TrySetResult();
            }, DispatcherPriority.Background);
        }, DispatcherPriority.Background);
    }

    private void MoveToVirtualScreenOrigin()
    {
        var screens = Screens.All;

        if (screens.Count == 0)
        {
            Position = new PixelPoint(0, 0);
            return;
        }

        var x = screens.Min(screen => screen.Bounds.X);
        var y = screens.Min(screen => screen.Bounds.Y);

        Position = new PixelPoint(x, y);
    }

    private void TryClose()
    {
        if (isClosing)
        {
            return;
        }

        isClosing = true;
        Close();
    }

    private void TransparentWindow_Deactivated(object? sender, EventArgs e)
    {
        TryClose();
    }

    private void TransparentWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        TryClose();
    }

    private void TransparentWindow_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        TryClose();
    }

    private void TransparentWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            TryClose();
        }
    }
}