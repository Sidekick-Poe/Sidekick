using CommunityToolkit.Maui.Alerts;
using Sidekick.Common.Platform;

namespace Sidekick.Maui.Services
{
    /// Implementation of the IViewInstance interface.
    public class ApplicationService : IApplicationService
    {
        public void Shutdown()
        {
            Application.Current.Quit();
            Environment.Exit(0);
        }

        public Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(string message)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make(message);
            _ = toast.Show(cancellationTokenSource.Token);
        }
    }
}
