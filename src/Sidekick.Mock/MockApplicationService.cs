using System;
using System.Threading.Tasks;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockApplicationService : IApplicationService
    {
        public Task<bool> OpenConfirmationModal(string message)
        {
            return Task.FromResult(false);
        }

        public void Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
