using Sidekick.Common.Extensions;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui.Dialogs;

public class DialogService(IViewLocator viewLocator) : ISidekickDialogs
{
    private TaskCompletionSource<bool>? ConfirmationResult { get; set; }

    private TaskCompletionSource<bool>? OkResult { get; set; }

    /// <inheritdoc/>
    public Task<bool> OpenConfirmationModal(string message)
    {
        SetConfirmationResult(false);

        _ = Task.Factory.StartNew(async () =>
        {
            await viewLocator.Open(SidekickViewType.Modal, $"/dialog/confirm/{message.EncodeBase64Url()}");
        });

        ConfirmationResult = new TaskCompletionSource<bool>();
        return ConfirmationResult.Task;
    }

    /// <inheritdoc/>
    public Task<bool> OpenOkModal(string message)
    {
        SetOkResult(false);

        _ = Task.Factory.StartNew(async () =>
        {
            await viewLocator.Open(SidekickViewType.Modal, $"/dialog/ok/{message.EncodeBase64Url()}");
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
