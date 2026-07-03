using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Dialogs;
using Sidekick.Common.Ui.Localization;

namespace Sidekick.Avalonia;

public partial class DialogWindow : Window
{
    private readonly TaskCompletionSource<DialogProvider.Result> taskCompletionSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private bool resultSet;

    public DialogWindow(DialogProvider.Type type, string message)
    {
        InitializeComponent();

        Type = type;
        Message = message;

        MessageTextBlock.Text = message;

        var resources = App.ServerAppHost?.Application.Services.GetService<IStringLocalizer<UiResources>>();
        switch (type)
        {
            case DialogProvider.Type.Ok:
                CancelButton.IsVisible = false;
                ConfirmButton.Content = resources?["Ok"] ?? "Ok";
                break;

            case DialogProvider.Type.Confirmation:
                CancelButton.IsVisible = true;
                CancelButton.Content = resources?["Cancel"] ?? "Cancel";
                ConfirmButton.Content = resources?["Confirm"] ?? "Confirm";
                break;
        }

        Closed += DialogWindow_Closed;
        Opened += DialogWindow_Opened;
    }

    public DialogProvider.Type Type { get; }

    public string Message { get; }

    public Task<DialogProvider.Result> Task => taskCompletionSource.Task;

    private void ConfirmButton_Click(object? sender, RoutedEventArgs e)
    {
        var result = Type == DialogProvider.Type.Confirmation
            ? DialogProvider.Result.Confirmed
            : DialogProvider.Result.Closed;

        SetResultAndClose(result);
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        SetResultAndClose(DialogProvider.Result.Closed);
    }

    private void DialogWindow_Closed(object? sender, EventArgs e)
    {
        SetResult(DialogProvider.Result.Closed);
    }

    private void SetResultAndClose(DialogProvider.Result result)
    {
        SetResult(result);
        Close();
    }

    private void SetResult(DialogProvider.Result result)
    {
        if (resultSet)
        {
            return;
        }

        resultSet = true;
        taskCompletionSource.TrySetResult(result);
    }

    private void DialogWindow_Opened(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Topmost = false;
            Topmost = true;

            Activate();
            Focus();

            ConfirmButton.Focus();
        }, DispatcherPriority.ApplicationIdle);
    }
}
