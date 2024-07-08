using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Extensions;

namespace Sidekick.Common.Blazor.Dialogs;

public class DialogService(IViewLocator viewLocator) : ISidekickDialogs
{
    private TaskCompletionSource<bool>? ConfirmationResult { get; set; }

    /// <inheritdoc/>
    public Task<bool> OpenConfirmationModal(string message)
    {
        SetConfirmationResult(false);

        _ = Task.Factory.StartNew(
            async () =>
            {
                await viewLocator.Open($"/dialog/confirm/{message.EncodeBase64Url()}");
            });

        ConfirmationResult = new TaskCompletionSource<bool>();
        return ConfirmationResult.Task;
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
}
