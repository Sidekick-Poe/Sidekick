using Sidekick.Common.Blazor.Views;

namespace Sidekick.Common.Blazor.Dialogs
{
    public class DialogService : ISidekickDialogs
    {
        private readonly IViewLocator viewLocator;

        public DialogService(IViewLocator viewLocator)
        {
            this.viewLocator = viewLocator;
        }

        private TaskCompletionSource<bool>? ConfirmationResult { get; set; }

        /// <inheritdoc/>
        public Task<bool> OpenConfirmationModal(string message)
        {
            SetConfirmationResult(false);

            _ = Task.Factory.StartNew(async () =>
            {
                await viewLocator.Open($"/dialog/confirm/{message}");
            });

            ConfirmationResult = new();
            return ConfirmationResult.Task;
        }

        internal void SetConfirmationResult(bool result)
        {
            if (ConfirmationResult != null)
            {
                ConfirmationResult.SetResult(result);
                ConfirmationResult = null;
            }
        }
    }
}
