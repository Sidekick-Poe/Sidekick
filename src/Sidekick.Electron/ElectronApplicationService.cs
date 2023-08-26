using ElectronNET.API.Entities;
using Sidekick.Common.Platform;

namespace Sidekick.Electron
{
    public class ElectronApplicationService : IApplicationService
    {
        public void Shutdown()
        {
            ElectronNET.API.Electron.App.Exit();
        }
    }
}
