using ElectronNET.API.Entities;
using Sidekick.Common.Platform;

namespace Sidekick.Electron
{
    public class ElectronApplicationService : IApplicationService
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public ElectronApplicationService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> OpenConfirmationModal(string message)
        {
            var options = new MessageBoxOptions(message)
            {
                Buttons = new[] { "Yes", "No" },
                CancelId = 1,
                DefaultId = 0,
                Title = "Confirmation required.",
                Icon = Path.Combine(webHostEnvironment.ContentRootPath, "Assets/icon.png"),
            };

            var result = await ElectronNET.API.Electron.Dialog.ShowMessageBoxAsync(options);
            if (result.Response == 0)
            {
                return true;
            }

            return false;
        }

        public void Shutdown()
        {
            ElectronNET.API.Electron.App.Exit();
        }
    }
}
