using System;
using System.Threading.Tasks;
using Sidekick.Common;

namespace Sidekick.Mock
{
    public class MockAppService : IAppService
    {
        public Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            return Task.CompletedTask;
        }

        public Task OpenNotification(string message, string title = null)
        {
            return Task.CompletedTask;
        }

        public void Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}