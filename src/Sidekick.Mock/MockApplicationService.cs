using System;
using System.Threading.Tasks;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockApplicationService : IApplicationService
    {
        public void Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
