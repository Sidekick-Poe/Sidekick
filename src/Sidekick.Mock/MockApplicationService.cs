using System;
using System.Threading.Tasks;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockApplicationService : IApplicationService
    {
        public Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            return Task.CompletedTask;
        }

        public void ShowToast(string message)
        {
        }

        public void Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
