using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Dialogs;
using Sidekick.Common.Ui.Localization;

namespace Sidekick.Wpf;

public partial class PopupWindow : Window
{
    private readonly IServiceProvider serviceProvider;
    private readonly TaskCompletionSource<DialogProvider.Result> taskCompletionSource = new();
    private bool resultSet;

    public PopupWindow(IServiceProvider serviceProvider, DialogProvider.Type type, string message)
    {
        this.serviceProvider = serviceProvider;
        InitializeComponent();

        Type = type;
        Message = message;

        MessageTextBlock.Text = message;

        ConfigureButtons(type);

        Closed += DialogWindow_Closed;
        Show();
    }

    public DialogProvider.Type Type { get; }

    public string Message { get; }

    public Task<DialogProvider.Result> Task => taskCompletionSource.Task;

    private void ConfigureButtons(DialogProvider.Type popupType)
    {
        var resources = serviceProvider.GetRequiredService<IStringLocalizer<UiResources>>();

        switch (popupType)
        {
            case DialogProvider.Type.Ok:
                CancelButton.Visibility = Visibility.Collapsed;
                ConfirmButton.Content = resources["Ok"];
                break;

            case DialogProvider.Type.Confirmation:
                CancelButton.Visibility = Visibility.Visible;
                CancelButton.Content = resources["Cancel"];
                ConfirmButton.Content = resources["Confirm"];
                break;
        }
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        var result = Type == DialogProvider.Type.Confirmation
            ? DialogProvider.Result.Confirmed
            : DialogProvider.Result.Closed;

        SetResultAndClose(result);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
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
        taskCompletionSource.SetResult(result);
    }
}
