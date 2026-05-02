using Sidekick.Common.Extensions;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui.Dialogs;

public class DialogService(IViewLocator viewLocator) : ISidekickDialogs
{
    private TaskCompletionSource<bool>? ConfirmationResult { get; set; }

    private TaskCompletionSource<bool>? OkResult { get; set; }

    /// <inheritdoc/>
    public Task<bool> OpenConfirmationDialog(string message)
    {
        SetConfirmationResult(false);

        _ = Task.Factory.StartNew(() =>
        {
            viewLocator.Open($"/dialog/confirm/{message.EncodeBase64Url()}", alwaysOnTop: false);
        });

        ConfirmationResult = new TaskCompletionSource<bool>();
        return ConfirmationResult.Task;
    }

    /// <inheritdoc/>
    public Task<bool> OpenOkDialog(string message)
    {
        SetOkResult(false);

        _ = Task.Factory.StartNew(() =>
        {
            viewLocator.Open($"/dialog/ok/{message.EncodeBase64Url()}", alwaysOnTop: false);
        });

        OkResult = new TaskCompletionSource<bool>();
        return OkResult.Task;
    }

    internal void SetConfirmationResult(bool result)
    {
        if (ConfirmationResult == null)
        {
            return;
        }

        ConfirmationResult.SetResult(result);
        ConfirmationResult = null;
    }

    internal void SetOkResult(bool result)
    {
        if (OkResult == null)
        {
            return;
        }

        OkResult.SetResult(result);
        OkResult = null;
    }
}
