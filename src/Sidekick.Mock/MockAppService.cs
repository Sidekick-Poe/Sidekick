using System;
using System.Threading.Tasks;
using Sidekick.Common;
using Sidekick.Common.Platform.Tray;

namespace Sidekick.Mock
{
    public class MockAppService : IAppService
    {
        private readonly ITrayProvider trayProvider;
        public MockAppService(ITrayProvider trayProvider)
        {
            this.trayProvider = trayProvider;
        }

        public Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            return Task.CompletedTask;
        }

        public Task OpenNotification(string message, string title = null)
        {
            trayProvider.SendNotification(message, title);

            return Task.CompletedTask;
        }

        public void Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
