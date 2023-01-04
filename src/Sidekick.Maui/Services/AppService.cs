using Sidekick.Common;

namespace Sidekick.Maui.Services
{
    /// Implementation of the IViewInstance interface.
    public class AppService : IAppService
    {
        public void Shutdown()
        {
            Application.Current.Quit();
        }

        public Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            throw new NotImplementedException();
        }
    }
}
